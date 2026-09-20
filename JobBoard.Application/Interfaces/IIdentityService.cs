using JobBoard.Application.DTOs;
using JobBoard.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobBoard.Application.Interfaces
{
    public interface IIdentityService
    {
        public Task CreateUser(string Email, string Password, UserRole Role);
    }
}
