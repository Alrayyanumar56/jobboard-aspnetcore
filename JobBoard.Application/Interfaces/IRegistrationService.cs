using JobBoard.Application.DTOs;

namespace JobBoard.Application.Interfaces;

public interface IRegistrationService
{
    Task RegisterUser(RegisterRequest  registerRequest);
}