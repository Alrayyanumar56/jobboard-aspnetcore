using JobBoard.Application.DTOs;
using JobBoard.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobBoard.Application.Services
{
    public class LoginService : ILoginService
    {
        private readonly IIdentityService identityService;

        public LoginService(IIdentityService identityService) {
            this.identityService = identityService;
        }
        public  Task<LoginResponse> LoginUser(LoginRequest loginRequest)
        {
            return identityService.LoginUser(loginRequest.Email, loginRequest.Password);

        }
    }
}
