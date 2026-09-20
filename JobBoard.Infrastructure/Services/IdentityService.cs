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
       public Task CreateUser(String Email, String Password, UserRole Role)
        {
            throw new NotImplementedException();
        }
    }
}
