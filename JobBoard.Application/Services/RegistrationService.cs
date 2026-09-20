using System.Diagnostics;
using JobBoard.Application.DTOs;
using JobBoard.Domain.Entities;
using Microsoft.AspNetCore.Identity;
namespace JobBoard.Application.Services;
using JobBoard.Application.Interfaces;

public class RegistrationService:IRegistrationService
{
    private readonly UserManager<> _userManager;
    RegistrationService(UserManager<ApplicationUser> _userManager)
    {
        this._userManager=_userManager;
    }

    public Task RegisterUser(RegisterRequest registerRequest)
    {
        throw new NotImplementedException();
    }
}