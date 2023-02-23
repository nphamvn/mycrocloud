using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;

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
    private IConstraint ResolveConstraint(string constraint)
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
            return (IConstraint)CreateConstraint(parameterPolicyType, argumentString);
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
    private static IConstraint CreateConstraint(Type type, string argumentString)
    {
        ConstructorInfo activationConstructor = null;
        object[] parameters = null;
        var constructors = type.GetConstructors();
        // If there is only one constructor and it has a single parameter, pass the argument string directly
        // This is necessary for the Regex RouteConstraint to ensure that patterns are not split on commas.
        if (constructors.Length == 1 && GetNonConvertableParameterTypeCount(constructors[0].GetParameters()) == 1)
        {
            activationConstructor = constructors[0];
            parameters = ConvertArguments(activationConstructor.GetParameters(), new string[] { argumentString });
        }
        else
        {
            var arguments = argumentString?.Split(',', StringSplitOptions.TrimEntries) ?? Array.Empty<string>();
            var matchingConstructors = constructors
                .Where(ci => GetNonConvertableParameterTypeCount(ci.GetParameters()) == arguments.Length)
                .OrderByDescending(ci => ci.GetParameters().Length)
                .ToArray();
            if (matchingConstructors.Length == 0)
            {
                throw new RouteCreationException("No constructor found");
            }
            else
            {
                // When there are multiple matching constructors, choose the one with the most service arguments
                if (matchingConstructors.Length == 1
                    || matchingConstructors[0].GetParameters().Length > matchingConstructors[1].GetParameters().Length)
                {
                    activationConstructor = matchingConstructors[0];
                }
                else
                {
                    throw new RouteCreationException("Constructors are ambiguous");
                }
                parameters = ConvertArguments(activationConstructor.GetParameters(), arguments);
            }
        }
        return (IConstraint)activationConstructor.Invoke(parameters);
    }

    private static int GetNonConvertableParameterTypeCount(ParameterInfo[] parameters)
    {
        var count = 0;
        for (var i = 0; i < parameters.Length; i++)
        {
            if (typeof(IConvertible).IsAssignableFrom(parameters[i].ParameterType))
            {
                count++;
            }
        }

        return count;
    }
    private static object[] ConvertArguments(ParameterInfo[] parameterInfos, string[] arguments)
    {
        var parameters = new object[parameterInfos.Length];
        var argumentPosition = 0;
        for (var i = 0; i < parameterInfos.Length; i++)
        {
            var parameter = parameterInfos[i];
            var parameterType = parameter.ParameterType;
            parameters[i] = Convert.ChangeType(arguments[argumentPosition], parameterType, CultureInfo.InvariantCulture);
            argumentPosition++;
        }

        return parameters;
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
