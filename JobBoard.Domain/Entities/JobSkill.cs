namespace JobBoard.Domain.Entities
{
    // Link row: this Job asks for this Skill. The (JobId, SkillId) pair is the primary key, so a job
    // can't list the same skill twice. Deleting the job deletes its links.
    public class JobSkill
    {
        public int JobId { get; set; }
        public int SkillId { get; set; }
    }
}
