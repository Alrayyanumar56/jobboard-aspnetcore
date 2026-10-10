using System;
using System.Collections.Generic;
using System.Text;
using JobBoard.Application.Interfaces;
using JobBoard.Application.DTOs;
using JobBoard.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using JobBoard.Infrastructure.Data;
namespace JobBoard.Infrastructure.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> userManager;
        private readonly IJWTService jWTService;

        public IdentityService(UserManager<ApplicationUser> userManager,IJWTService jWTService)
        {
            this.userManager = userManager;
            this.jWTService = jWTService;
        }
        public async Task<IdentityResult> CreateUser(String Email, String Password, UserRole Role)
        {
            ApplicationUser user = new ApplicationUser();
            user.UserName = Email;
            user.Email = Email;
            var result = await userManager.CreateAsync(user, Password);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, Role.ToString());
            }
            return result;
        }

        public async Task<LoginResponse> LoginUser(String Email, String Password)
        {
            ApplicationUser applicationUser = new ApplicationUser();
            applicationUser.Email = Email;
            var EmailResult = await userManager.FindByEmailAsync(applicationUser.Email);
            if (EmailResult == null)
            {
                return null;
            }
            else { 
                bool PassResult = await userManager.CheckPasswordAsync(EmailResult, Password);
                if (PassResult == false)
                { return null; }
                else
                {
                    var UserRole = await userManager.GetRolesAsync(EmailResult);
                    var Role = UserRole.FirstOrDefault();
                    var UserId = EmailResult.Id;
                    var token = jWTService.GetJWT(UserId, Email, Role, DateTime.UtcNow.AddHours(1));
                    LoginResponse loginResponse = new LoginResponse();
                    loginResponse.Token = token;
                    loginResponse.Email = Email;
                    loginResponse.Role = Enum.Parse<UserRole>(Role);
                    return loginResponse;
                }
            }
            return null;
        }
    }
}
