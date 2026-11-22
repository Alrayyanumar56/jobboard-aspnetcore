using JobBoard.Domain.Enums;

namespace JobBoard.Application.DTOs
{
    // Response of GET /api/Auth/me: who the caller is, read from their validated token.
    // Lets a frontend restore its session after a page refresh without decoding the JWT itself.
    public class CurrentUserResponse
    {
        public string UserId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public UserRole Role { get; set; }
    }
}
