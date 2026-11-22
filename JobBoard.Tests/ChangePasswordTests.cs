using JobBoard.Application.Auth.ChangePassword;
using JobBoard.Application.DTOs;
using JobBoard.Tests.Fakes;

namespace JobBoard.Tests
{
    public class ChangePasswordTests
    {
        private const string UserA = "user-a";
        private const string UserB = "user-b";

        private readonly FakeIdentityService identity = new();

        public ChangePasswordTests()
        {
            identity.Passwords[UserA] = "Old@12345";
            identity.Passwords[UserB] = "Other@12345";
        }

        private Task<ChangePasswordResult> Change(string userId, string current, string next) =>
            new ChangePasswordHandler(identity).Handle(new ChangePasswordRequest { CurrentPassword = current, NewPassword = next }, userId);

        [Fact]
        public async Task Correct_current_password_changes_only_the_callers_password()
        {
            var result = await Change(UserA, "Old@12345", "New@12345");

            Assert.Equal(ChangePasswordOutcome.Changed, result.Outcome);
            Assert.Equal("New@12345", identity.Passwords[UserA]);
            Assert.Equal("Other@12345", identity.Passwords[UserB]);
        }

        [Fact]
        public async Task Wrong_current_password_is_rejected_on_CurrentPassword()
        {
            var result = await Change(UserA, "Wrong@12345", "New@12345");

            Assert.Equal(ChangePasswordOutcome.Rejected, result.Outcome);
            Assert.True(result.Errors.ContainsKey("CurrentPassword"));
            Assert.Equal("Old@12345", identity.Passwords[UserA]);
        }

        [Fact]
        public async Task New_password_equal_to_current_is_rejected_on_NewPassword()
        {
            var result = await Change(UserA, "Old@12345", "Old@12345");

            Assert.Equal(ChangePasswordOutcome.Rejected, result.Outcome);
            Assert.True(result.Errors.ContainsKey("NewPassword"));
        }

        [Fact]
        public async Task Deleted_user_gives_UserNotFound()
        {
            var result = await Change("gone", "Old@12345", "New@12345");

            Assert.Equal(ChangePasswordOutcome.UserNotFound, result.Outcome);
        }
    }
}
