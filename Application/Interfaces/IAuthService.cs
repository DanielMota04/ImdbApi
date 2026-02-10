using Application.DTOs.Request.Auth;
using Application.DTOs.Response.Auth;

namespace Application.Interfaces
{
    public interface IAuthService
    {
        public Task<AuthResponseDTO> RegisterAsync(AuthRegisterRequestDTO dto);
        public Task<string?> LoginAsync(AuthLoginRequestDTO dto);
    }
}
