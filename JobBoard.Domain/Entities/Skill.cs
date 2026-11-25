using JobBoard.Domain.Rules;
using System.ComponentModel.DataAnnotations;

namespace JobBoard.Domain.Entities
{
    // One skill in the shared vocabulary ("C#", "ASP.NET Core", "SQL"). Jobs and candidate profiles
    // link to the same rows (JobSkill, CandidateProfileSkill), which is what lets them be matched later.
    public class Skill
    {
        public int Id { get; set; }

        // As first written by whoever introduced the skill, after SkillNameRules.Clean.
        [Required]
        [MaxLength(SkillNameRules.MaxLength)]
        public string Name { get; set; } = string.Empty;

        // SkillNameRules.Normalize(Name). Unique: "asp.net core" and "ASP.NET Core" are one skill.
        [Required]
        [MaxLength(SkillNameRules.MaxLength)]
        public string NormalizedName { get; set; } = string.Empty;
    }
}
