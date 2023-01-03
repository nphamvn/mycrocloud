using System.Text.RegularExpressions;

namespace MockServer.Core.Services;

public class TemplateUtility
{
    public Dictionary<string, string> GetExpressions(string template)
    {
        string pattern = @"@{(.*?)}";
        RegexOptions options = RegexOptions.Singleline;
        MatchCollection matches = Regex.Matches(template, pattern, options);
        Dictionary<string, string> ret = new();
        int i = 0;
        foreach (Match match in matches)
        {
            if (match.Success)
            {
                string exp = match.Groups[1].Value;
                ret.Add($"Expression_{i}", exp);
                i++;
            }
        }
        return ret;
    }
}
