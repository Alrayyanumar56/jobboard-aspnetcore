using JobBoard.Application.DTOs;
using JobBoard.Domain.Enums;

namespace JobBoard.Application.Interfaces
{
    public interface IIdentityService
    {
        // Creates the user AND assigns the role, or neither: if the role can't be assigned the new
        // user is removed again, so a user without a role never stays in the database.
        public Task<RegistrationResult> CreateUser(string Email, string Password, UserRole Role);

        // Null when the email/password pair is wrong (or the account has no usable role).
        public Task<LoginResponse?> LoginUser(String Email, String Password);
    }
}
