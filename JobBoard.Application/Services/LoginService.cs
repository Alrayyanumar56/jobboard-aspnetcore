using JobBoard.Application.DTOs;
using JobBoard.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobBoard.Application.Services
{
    internal class LoginService : ILoginService
    {
        private readonly IIdentityService identityService;

        public LoginService(IIdentityService identityService) {
            this.identityService = identityService;
        }
        public  Task<LoginResponse> LoginUser(LoginRequest loginRequest)
        {
            throw new NotImplementedException();
        }
    }
}
