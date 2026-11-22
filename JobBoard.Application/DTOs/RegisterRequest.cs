using System.ComponentModel.DataAnnotations;

namespace JobBoard.Application.DTOs;
using JobBoard.Domain.Enums;
public class RegisterRequest
{
    [EmailAddress]
    [Required]
    public string Email { get; set; } = string.Empty;
    [Required]
    public String Password { get; set; } = string.Empty;

    // Nullable + [Required] so an omitted role is a 400, not a silent default of Employer (0).
    // [EnumDataType] rejects undefined values such as 5, which used to create a user with no role.
    [Required]
    [EnumDataType(typeof(UserRole))]
    public UserRole? Role { get; set; }
}
