using System;
using System.Collections.Generic;
using System.Text;

namespace JobBoard.Application.Interfaces
{
    public interface IJWTService
    {
        public string GetJWT(string UserId, string Email, string Role, DateTime Expiration);
    }
}
