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

       public  IdentityService(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
        }
       public async Task<IdentityResult> CreateUser(String Email, String Password, UserRole Role)
        {
            ApplicationUser user = new ApplicationUser();
            user.UserName= Email;
            user.Email= Email;
           var result= await userManager.CreateAsync(user,Password);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, Role.ToString());
            }
            return result;
        }
    }
}
