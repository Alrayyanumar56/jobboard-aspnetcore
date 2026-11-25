namespace JobBoard.Domain.Entities
{
    // Link row: this candidate profile has this Skill. The (CandidateProfileId, SkillId) pair is the
    // primary key, so a profile can't list the same skill twice. Deleting the profile deletes its links.
    public class CandidateProfileSkill
    {
        public int CandidateProfileId { get; set; }
        public int SkillId { get; set; }
    }
}
