using JobBoard.Application.DTOs;

namespace JobBoard.Application.Interfaces
{
    public interface ILoginService
    {
       // Null means the credentials were not accepted; the controller turns that into a 401.
       Task<LoginResponse?> LoginUser(LoginRequest loginRequest);
    }
}
