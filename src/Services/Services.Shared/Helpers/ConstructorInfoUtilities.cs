using System.Globalization;
using System.Reflection;

namespace MycroCloud.Shared.Helpers;

public class ConstructorInfoUtilities
{
    public static T CreateInstance<T>(Type type, string argumentString)
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
                throw new Exception("No constructor found");
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
                    throw new Exception("Constructors are ambiguous");
                }
                parameters = ConvertArguments(activationConstructor.GetParameters(), arguments);
            }
        }
        return (T)activationConstructor.Invoke(parameters);
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
}
