using JobBoard.Application.DTOs;
using JobBoard.Domain.Entities;

namespace JobBoard.Application.Interfaces
{
    public interface ISkillRepository
    {
        // One Skill per name, creating the ones that don't exist yet. The names must already have been
        // through SkillNameRules.CleanList (cleaned, no duplicates). Safe when two requests create the
        // same new skill at once: the unique index rejects one insert and the existing row is used.
        public Task<List<Skill>> GetOrCreate(IReadOnlyCollection<string> cleanedNames);

        // Up to `take` skills whose name contains `search` (all skills when blank), alphabetical.
        public Task<List<SkillResponse>> Search(string? search, int take);

        // The skill names on one candidate profile, alphabetical. Empty when it has none.
        public Task<List<string>> GetCandidateSkillNames(int candidateProfileId);
    }
}
