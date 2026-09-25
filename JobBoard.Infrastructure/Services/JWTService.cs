using JobBoard.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JobBoard.Infrastructure.Services
{
    internal class JWTService : IJWTService
    {
        private readonly JwtSecurityTokenHandler jwtSecurityTokenHandler;

        JWTService(JwtSecurityTokenHandler jwtSecurityTokenHandler) {
            this.jwtSecurityTokenHandler = jwtSecurityTokenHandler;
        }
        public Task<string> GetJWTAsync(int UserID, string Email, string Role, DateTime Expiration)
        {
            JwtSecurityToken token = new JwtSecurityToken()

        }
    }
}
