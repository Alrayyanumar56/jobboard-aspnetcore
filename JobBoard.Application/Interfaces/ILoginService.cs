using JobBoard.Application.DTOs;

namespace JobBoard.Application.Interfaces
{
    public interface ILoginService
    {
       Task<LoginResponse> LoginUser(LoginRequest loginRequest);
    }
}
