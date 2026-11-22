using JobBoard.Application.Auth.ChangePassword;
using JobBoard.Application.DTOs;
using JobBoard.Domain.Enums;

namespace JobBoard.Application.Interfaces
{
    public interface IIdentityService
    {
        // Creates the user AND assigns the role, or neither: if the role can't be assigned the new
        // user is removed again, so a user without a role never stays in the database.
        public Task<RegistrationResult> CreateUser(string Email, string Password, UserRole Role);

        // Null when the email/password pair is wrong, the account is locked out after too many failed
        // attempts, or it has no usable role. Callers can't tell which, so emails are never revealed.
        public Task<LoginResponse?> LoginUser(String Email, String Password);

        // Checks currentPassword, then sets newPassword (Identity's password rules apply).
        public Task<ChangePasswordResult> ChangePassword(string userId, string currentPassword, string newPassword);
    }
}
