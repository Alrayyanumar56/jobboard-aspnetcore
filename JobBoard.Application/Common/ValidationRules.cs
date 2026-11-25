using JobBoard.Domain.Rules;
using System.ComponentModel.DataAnnotations;

namespace JobBoard.Application.Common
{
    // Checks shared by several request commands, called from their IValidatableObject.Validate so
    // [ApiController] turns a failure into a normal 400 ValidationProblem.
    internal static class ValidationRules
    {
        // An absolute http:// or https:// address. [Url] alone would also accept ftp:// and other schemes.
        public static bool IsHttpUrl(string? value) =>
            Uri.TryCreate(value?.Trim(), UriKind.Absolute, out var uri)
            && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);

        public static IEnumerable<ValidationResult> ValidateHttpUrl(string? value, string memberName)
        {
            if (!string.IsNullOrWhiteSpace(value) && !IsHttpUrl(value))
            {
                yield return new ValidationResult(
                    $"{memberName} must be an absolute http:// or https:// address.",
                    new[] { memberName });
            }
        }

        // A skills list: at most SkillNameRules.MaxSkillsPerItem different skills, each at most
        // SkillNameRules.MaxLength characters after cleaning. Blank entries and repeats are ignored.
        public static IEnumerable<ValidationResult> ValidateSkills(List<string>? skills, string memberName)
        {
            var cleaned = SkillNameRules.CleanList(skills);
            if (cleaned.Count > SkillNameRules.MaxSkillsPerItem)
            {
                yield return new ValidationResult(
                    $"At most {SkillNameRules.MaxSkillsPerItem} different skills are allowed.",
                    new[] { memberName });
            }
            foreach (var name in cleaned.Where(n => n.Length > SkillNameRules.MaxLength))
            {
                yield return new ValidationResult(
                    $"Skill '{name[..20]}...' is longer than {SkillNameRules.MaxLength} characters.",
                    new[] { memberName });
            }
        }
    }
}
