using System.Text.RegularExpressions;

namespace JobBoard.Domain.Rules
{
    // How a skill name typed by a user becomes one entry in the shared skill vocabulary.
    // Pure logic, unit-tested, used by both job and candidate-profile skills.
    public static class SkillNameRules
    {
        public const int MaxLength = 50;
        public const int MaxSkillsPerItem = 20;

        private static readonly Regex Whitespace = new(@"\s+", RegexOptions.Compiled);

        // Trims and collapses inner whitespace: "  ASP.NET   Core " -> "ASP.NET Core".
        // Null when nothing is left.
        public static string? Clean(string? raw)
        {
            if (string.IsNullOrWhiteSpace(raw))
            {
                return null;
            }
            return Whitespace.Replace(raw.Trim(), " ");
        }

        // The comparison key stored in Skill.NormalizedName: case doesn't make a different skill.
        public static string Normalize(string cleanedName) => cleanedName.ToUpperInvariant();

        // Cleans every name, drops blanks, and keeps the FIRST spelling of each skill
        // ("C#", "c#" -> "C#"), in the order given.
        public static List<string> CleanList(IEnumerable<string?>? rawNames)
        {
            var result = new List<string>();
            var seen = new HashSet<string>();
            foreach (var raw in rawNames ?? [])
            {
                var cleaned = Clean(raw);
                if (cleaned != null && seen.Add(Normalize(cleaned)))
                {
                    result.Add(cleaned);
                }
            }
            return result;
        }
    }
}
