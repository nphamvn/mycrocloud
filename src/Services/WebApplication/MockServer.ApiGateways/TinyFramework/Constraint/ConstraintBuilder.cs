using System.Diagnostics.CodeAnalysis;
using MockServer.Core.Helpers;

namespace MockServer.Api.TinyFramework;

public class ConstraintBuilder
{
    public ConstraintBuilder(IDictionary<string, Type> inlineConstraintMap)
    {
        _inlineConstraintMap = inlineConstraintMap;
    }
    private List<IConstraint> _constraints = new List<IConstraint>();
    public List<IConstraint> Build()
    {
        return _constraints;
    }
    public void AddResolvedConstraint(string constraintText)
    {
        var constraint = ResolveConstraint(constraintText);
        if (constraint != null)
        {
            Add(constraint);
        }
    }
    public void Add(IConstraint constraint)
    {
        _constraints.Add(constraint);
    }

    private readonly IDictionary<string, Type> _inlineConstraintMap;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="constraint">A typical constraint looks like the following
    /// "exampleConstraint(arg1, arg2, 12)".</param>
    /// <returns></returns>
    public IConstraint ResolveConstraint(string constraint)
    {
        string argumentString;
        string key;
        var indexOfFirstOpenParens = constraint.IndexOf('(');
        if (indexOfFirstOpenParens >= 0 && constraint.EndsWith(')'))
        {
            key = constraint.Substring(0, indexOfFirstOpenParens);
            argumentString = constraint.Substring(
                indexOfFirstOpenParens + 1,
                constraint.Length - indexOfFirstOpenParens - 2);
        }
        else
        {
            key = constraint;
            argumentString = null;
        }
        if (!_inlineConstraintMap.TryGetValue(key, out var parameterPolicyType))
        {
            return default;
        }
        if (!typeof(IConstraint).IsAssignableFrom(parameterPolicyType))
        {
            if (!typeof(IConstraint).IsAssignableFrom(parameterPolicyType))
            {
                // Error if type is not a parameter policy
                throw new RouteCreationException("type is not a parameter policy");
            }

            // Return null if type is parameter policy but is not the exact type
            // This is used by IInlineConstraintResolver for backwards compatibility
            // e.g. looking for an IRouteConstraint but get a different IParameterPolicy type
            return default;
        }
        try
        {
            return ConstructorInfoUtilities.CreateInstance<IConstraint>(parameterPolicyType, argumentString);
        }
        catch (RouteCreationException)
        {
            throw;
        }
        catch (Exception exception)
        {
            throw new RouteCreationException(
                $"An error occurred while trying to create an instance of '{parameterPolicyType.FullName}'.",
                exception);
        }
    }
    public static IDictionary<string, Type> GetDefaultConstraintMap()
    {
        var defaults = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);
        // Type-specific constraints
        AddConstraint<IntConstraint>(defaults, "int");
        AddConstraint<BoolConstraint>(defaults, "bool");
        AddConstraint<DateTimeConstraint>(defaults, "datetime");
        AddConstraint<DecimalConstraint>(defaults, "decimal");
        AddConstraint<DoubleConstraint>(defaults, "double");
        AddConstraint<FloatConstraint>(defaults, "float");
        AddConstraint<GuidConstraint>(defaults, "guid");
        AddConstraint<LongConstraint>(defaults, "long");

        // Length constraints
        AddConstraint<MinLengthConstraint>(defaults, "minlength");
        AddConstraint<MaxLengthConstraint>(defaults, "maxlength");
        AddConstraint<LengthConstraint>(defaults, "length");

        // Min/Max value constraints
        AddConstraint<MinConstraint>(defaults, "min");
        AddConstraint<MaxConstraint>(defaults, "max");
        AddConstraint<RangeConstraint>(defaults, "range");

        // Regex-based constraints
        AddConstraint<AlphaConstraint>(defaults, "alpha");
        AddConstraint<RegexInlineConstraint>(defaults, "regex");

        AddConstraint<RequiredConstraint>(defaults, "required");

        // Files
        AddConstraint<FileNameConstraint>(defaults, "file");
        AddConstraint<NonFileNameConstraint>(defaults, "nonfile");
        return defaults;
    }

    private static void AddConstraint<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TConstraint>(Dictionary<string, Type> constraintMap, string text) where TConstraint : IConstraint
    {
        constraintMap[text] = typeof(TConstraint);
    }
}
