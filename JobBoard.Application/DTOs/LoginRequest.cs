using System.ComponentModel.DataAnnotations;



namespace JobBoard.Application.DTOs
{
    public class LoginRequest
    {
        [Required]
        [EmailAddress]
        public String Email { get; set; }
        [Required]
        public String Password { get; set; }
    }
}
