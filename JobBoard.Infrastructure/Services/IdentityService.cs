using JobBoard.Application.Interfaces;
using JobBoard.Application.DTOs;
using JobBoard.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using JobBoard.Infrastructure.Data;
namespace JobBoard.Infrastructure.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IJWTService jWTService;

        public IdentityService(UserManager<ApplicationUser> userManager,IJWTService jWTService)
        {
            this.userManager = userManager;
            this.jWTService = jWTService;
        }
        public async Task<RegistrationResult> CreateUser(String Email, String Password, UserRole Role)
        {
            ApplicationUser user = new ApplicationUser();
            user.UserName = Email;
            user.Email = Email;
            var result = await userManager.CreateAsync(user, Password);
            if (!result.Succeeded)
            {
                return RegistrationResult.Failure(ToFieldErrors(result.Errors));
            }

            // CreateAsync and AddToRoleAsync are two separate saves. If the role can't be assigned,
            // remove the user again: a user with no role can never log in, and would block the email
            // from being registered properly later.
            var roleResult = await userManager.AddToRoleAsync(user, Role.ToString());
            if (!roleResult.Succeeded)
            {
                await userManager.DeleteAsync(user);
                return RegistrationResult.Failure(ToFieldErrors(roleResult.Errors));
            }

            return RegistrationResult.Success();
        }

        public async Task<LoginResponse?> LoginUser(String Email, String Password)
        {
            var user = await userManager.FindByEmailAsync(Email);
            if (user == null)
            {
                return null;
            }

            bool PassResult = await userManager.CheckPasswordAsync(user, Password);
            if (PassResult == false)
            {
                return null;
            }

            // A user must have exactly one of the public roles (Employer / JobSeeker). If the role is
            // missing or isn't one of those, refuse the login (401) instead of crashing with a 500.
            var UserRoles = await userManager.GetRolesAsync(user);
            var Role = UserRoles.FirstOrDefault();
            if (Role == null || !Enum.TryParse<UserRole>(Role, out var parsedRole))
            {
                return null;
            }

            var token = jWTService.GetJWT(user.Id, user.Email ?? Email, Role, DateTime.UtcNow.AddHours(1));
            LoginResponse loginResponse = new LoginResponse();
            loginResponse.Token = token;
            loginResponse.Email = user.Email ?? Email;
            loginResponse.Role = parsedRole;
            return loginResponse;
        }

        // Turns Identity's error list into "field -> messages" so the API can return a standard
        // ValidationProblem and the frontend can show each message under the right input.
        private static Dictionary<string, string[]> ToFieldErrors(IEnumerable<IdentityError> errors)
        {
            return errors
                .GroupBy(e => FieldFor(e.Code))
                .ToDictionary(g => g.Key, g => g.Select(e => e.Description).ToArray());
        }

        private static string FieldFor(string identityErrorCode)
        {
            if (identityErrorCode.StartsWith("Password", StringComparison.Ordinal))
            {
                return "Password";
            }
            if (identityErrorCode.Contains("Email", StringComparison.Ordinal) || identityErrorCode.Contains("UserName", StringComparison.Ordinal))
            {
                return "Email";
            }
            return "Registration";
        }
    }
}
