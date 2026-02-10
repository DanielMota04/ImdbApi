using ImdbApi.DTOs.Request.Auth;
using ImdbApi.DTOs.Response.Auth;

namespace ImdbApi.Interfaces.Services
{
    public interface IAuthService
    {
        public Task<AuthResponseDTO> RegisterAsync(AuthRegisterRequestDTO dto);
        public Task<string?> LoginAsync(AuthLoginRequestDTO dto);
    }
}
