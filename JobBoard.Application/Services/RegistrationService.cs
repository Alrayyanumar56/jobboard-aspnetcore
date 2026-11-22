using JobBoard.Application.DTOs;
namespace JobBoard.Application.Services;
using JobBoard.Application.Interfaces;

public class RegistrationService:IRegistrationService
{
    private readonly IIdentityService identityService;

    public RegistrationService(IIdentityService identityService)
    {
        this.identityService = identityService;
    }

    public async Task<RegistrationResult> RegisterUser(RegisterRequest registerRequest)
    {
        // Role is guaranteed non-null and defined here: [Required] + [EnumDataType] on RegisterRequest
        // are checked by [ApiController] before this method is ever called.
        var result = await identityService.CreateUser(registerRequest.Email.Trim(), registerRequest.Password, registerRequest.Role!.Value);
        return result;
    }
}
