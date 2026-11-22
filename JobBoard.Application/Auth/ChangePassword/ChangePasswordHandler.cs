using JobBoard.Application.DTOs;
using JobBoard.Application.Interfaces;

namespace JobBoard.Application.Auth.ChangePassword
{
    public class ChangePasswordHandler
    {
        private readonly IIdentityService identityService;

        public ChangePasswordHandler(IIdentityService identityService)
        {
            this.identityService = identityService;
        }

        // userId comes from the authenticated user's JWT (resolved in the controller), never from the
        // request, so a user can only ever change their OWN password. The current password is still
        // required: a stolen token alone must not be enough to take over the account.
        public async Task<ChangePasswordResult> Handle(ChangePasswordRequest request, string userId)
        {
            if (request.CurrentPassword == request.NewPassword)
            {
                return ChangePasswordResult.Rejected(new Dictionary<string, string[]>
                {
                    ["NewPassword"] = ["The new password must be different from the current password."]
                });
            }

            return await identityService.ChangePassword(userId, request.CurrentPassword, request.NewPassword);
        }
    }
}
