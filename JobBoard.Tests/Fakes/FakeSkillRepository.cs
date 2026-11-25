using JobBoard.Application.DTOs;
using JobBoard.Application.Interfaces;
using JobBoard.Domain.Entities;
using JobBoard.Domain.Rules;

namespace JobBoard.Tests.Fakes
{
    // In-memory skill vocabulary plus the two link tables, shared by the job and profile fakes so that
    // "the same skill" means the same id everywhere, as in the database.
    public class FakeSkillRepository : ISkillRepository
    {
        public List<Skill> Skills { get; } = new();
        public List<JobSkill> JobSkills { get; } = new();
        public List<CandidateProfileSkill> CandidateSkills { get; } = new();

        public Task<List<Skill>> GetOrCreate(IReadOnlyCollection<string> cleanedNames)
        {
            var result = new List<Skill>();
            foreach (var name in cleanedNames)
            {
                var key = SkillNameRules.Normalize(name);
                var skill = Skills.FirstOrDefault(s => s.NormalizedName == key);
                if (skill == null)
                {
                    skill = new Skill { Id = Skills.Count + 1, Name = name, NormalizedName = key };
                    Skills.Add(skill);
                }
                result.Add(skill);
            }
            return Task.FromResult(result);
        }

        public Task<List<SkillResponse>> Search(string? search, int take)
        {
            var cleaned = SkillNameRules.Clean(search);
            var key = cleaned == null ? null : SkillNameRules.Normalize(cleaned);
            return Task.FromResult(Skills
                .Where(s => key == null || s.NormalizedName.Contains(key))
                .OrderBy(s => s.Name, StringComparer.OrdinalIgnoreCase)
                .Take(take)
                .Select(s => new SkillResponse { Id = s.Id, Name = s.Name })
                .ToList());
        }

        public Task<List<string>> GetCandidateSkillNames(int candidateProfileId) =>
            Task.FromResult(NamesOf(CandidateSkills.Where(cs => cs.CandidateProfileId == candidateProfileId).Select(cs => cs.SkillId)));

        public List<string> JobSkillNames(int jobId) =>
            NamesOf(JobSkills.Where(js => js.JobId == jobId).Select(js => js.SkillId));

        public void ReplaceJobSkills(int jobId, IEnumerable<int> skillIds)
        {
            JobSkills.RemoveAll(js => js.JobId == jobId);
            JobSkills.AddRange(skillIds.Select(id => new JobSkill { JobId = jobId, SkillId = id }));
        }

        public void ReplaceCandidateSkills(int profileId, IEnumerable<int> skillIds)
        {
            CandidateSkills.RemoveAll(cs => cs.CandidateProfileId == profileId);
            CandidateSkills.AddRange(skillIds.Select(id => new CandidateProfileSkill { CandidateProfileId = profileId, SkillId = id }));
        }

        private List<string> NamesOf(IEnumerable<int> skillIds) =>
            skillIds.Select(id => Skills.Single(s => s.Id == id).Name).Order(StringComparer.OrdinalIgnoreCase).ToList();
    }
}
