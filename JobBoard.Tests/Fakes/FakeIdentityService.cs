using JobBoard.Application.Auth.ChangePassword;
using JobBoard.Application.DTOs;
using JobBoard.Application.Interfaces;
using JobBoard.Domain.Enums;

namespace JobBoard.Tests.Fakes
{
    // In-memory IIdentityService for the change-password handler: user id -> current password.
    // Identity's own password rules are not re-implemented; only the current-password check is.
    public class FakeIdentityService : IIdentityService
    {
        public Dictionary<string, string> Passwords { get; } = new();

        public Task<ChangePasswordResult> ChangePassword(string userId, string currentPassword, string newPassword)
        {
            if (!Passwords.TryGetValue(userId, out var stored))
            {
                return Task.FromResult(ChangePasswordResult.UserNotFound());
            }
            if (stored != currentPassword)
            {
                return Task.FromResult(ChangePasswordResult.Rejected(new Dictionary<string, string[]>
                {
                    ["CurrentPassword"] = ["Incorrect password."]
                }));
            }
            Passwords[userId] = newPassword;
            return Task.FromResult(ChangePasswordResult.Changed());
        }

        public Task<RegistrationResult> CreateUser(string Email, string Password, UserRole Role) =>
            throw new NotSupportedException();

        public Task<LoginResponse?> LoginUser(string Email, string Password) =>
            throw new NotSupportedException();
    }
}
