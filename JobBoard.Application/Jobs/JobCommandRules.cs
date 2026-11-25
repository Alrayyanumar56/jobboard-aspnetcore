using JobBoard.Application.Common;
using System.ComponentModel.DataAnnotations;

namespace JobBoard.Application.Jobs
{
    // Cross-field rules shared by CreateJobCommand and UpdateJobCommand. [ApiController] runs them
    // through IValidatableObject and answers 400, so the handlers only see valid commands.
    internal static class JobCommandRules
    {
        public static IEnumerable<ValidationResult> Validate(DateTimeOffset? applicationDeadline, List<string>? skills)
        {
            // A deadline in the past would hide the job the moment it is saved. An employer editing a job
            // whose deadline has passed must move the deadline forward or remove it.
            if (applicationDeadline.HasValue && applicationDeadline.Value <= DateTimeOffset.UtcNow)
            {
                yield return new ValidationResult(
                    "ApplicationDeadline must be in the future.",
                    new[] { "ApplicationDeadline" });
            }

            foreach (var result in ValidationRules.ValidateSkills(skills, "Skills"))
            {
                yield return result;
            }
        }
    }
}
