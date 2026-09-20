using JobBoard.Application.DTOs;
using Microsoft.AspNetCore.Identity;

namespace JobBoard.Application.Interfaces;

public interface IRegistrationService
{
    Task<IdentityResult> RegisterUser(RegisterRequest  registerRequest);
}