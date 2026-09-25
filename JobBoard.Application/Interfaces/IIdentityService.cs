using JobBoard.Application.DTOs;
using JobBoard.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobBoard.Application.Interfaces
{
    public interface IIdentityService
    {
        public Task<IdentityResult> CreateUser(string Email, string Password, UserRole Role);
        public Task<LoginResponse> LoginUser(String Email, String Password);
    }
}
