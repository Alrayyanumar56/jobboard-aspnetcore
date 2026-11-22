using System.ComponentModel.DataAnnotations;

namespace JobBoard.Application.DTOs
{
    // Body of POST /api/Auth/change-password. No user id or email: the caller comes from the token.
    public class ChangePasswordRequest
    {
        [Required]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required]
        public string NewPassword { get; set; } = string.Empty;
    }
}
