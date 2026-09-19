using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace JobBoard.Application.DTOs;
using JobBoard.Domain.Enums;
public class RegisterRequest
{
    [EmailAddress]
    [Required]
    public string Email { get; set; }
    [Required] 
    public String Password { get; set; }
    [Required]
    public UserRole  Role { get; set; }
}