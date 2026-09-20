using System.Diagnostics;
using JobBoard.Application.DTOs;
using JobBoard.Domain.Entities;
using Microsoft.AspNetCore.Identity;
namespace JobBoard.Application.Services;
using JobBoard.Application.Interfaces;

public class RegistrationService:IRegistrationService
{
    private readonly IIdentityService identityService;

    public RegistrationService(IIdentityService identityService)
    {
        this.identityService = identityService;
    }

    public async Task RegisterUser(RegisterRequest registerRequest)
    {
       await identityService.CreateUser(registerRequest.Email, registerRequest.Password, registerRequest.Role);
    }
}