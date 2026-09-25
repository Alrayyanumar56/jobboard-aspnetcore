using System;
using System.Collections.Generic;
using System.Text;

namespace JobBoard.Application.Interfaces
{
    public interface IJWTService
    {
        public Task<String> GetJWTAsync(int UserID, String Email,String Role, DateTime Expiration);
    }
}
