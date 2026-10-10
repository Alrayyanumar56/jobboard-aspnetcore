using JobBoard.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace JobBoard.Infrastructure.Services
{
    public class JWTService : IJWTService
    {

        private readonly IConfiguration configuration;

        public JWTService(IConfiguration configuration) {
            this.configuration = configuration;
        }
        public string GetJWT(string UserId, string Email, string Role, DateTime Expiration)
        {
            Claim claim=new Claim(ClaimTypes.NameIdentifier, UserId);
            Claim claim1=new Claim(ClaimTypes.Email, Email);
            Claim claim2=new Claim(ClaimTypes.Role, Role);
            List<Claim> claims = new List<Claim> { claim, claim1, claim2 };
            DateTime expries = Expiration;
            DateTime notbefore = DateTime.UtcNow;
            string signingCred = configuration["Jwt:SecretKey"];
            byte[] keybytes=Convert.FromBase64String(signingCred);
            SymmetricSecurityKey symmetricSecurityKey = new SymmetricSecurityKey(keybytes);
            SigningCredentials signingCredential = new SigningCredentials(symmetricSecurityKey,SecurityAlgorithms.HmacSha256);
            JwtSecurityToken jwtSecurityToken = new JwtSecurityToken(issuer: null, audience: null, claims, notbefore, expries, signingCredential);
            JwtSecurityTokenHandler jwtSecurityTokenHandler= new JwtSecurityTokenHandler();
            var result = jwtSecurityTokenHandler.WriteToken(jwtSecurityToken);
            return result;
        }
    }
}
