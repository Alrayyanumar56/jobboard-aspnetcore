namespace JobBoard.Application.DTOs
{
    // One entry of GET /api/Skill?search=..., for autocomplete. Clients send skills back by Name.
    public class SkillResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
