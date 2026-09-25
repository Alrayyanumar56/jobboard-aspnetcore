using JobBoard.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace JobBoard.Application.DTOs
{
    public class LoginResponse
    {
        [Required]
        public String Token{ get; set; }
        [Required]
        [EmailAddress]
        public String Email{ get; set; }
        [Required]
        
        public UserRole Role{ get; set; }
    }
}
