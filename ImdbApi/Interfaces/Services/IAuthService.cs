using ImdbApi.DTOs.Request;
using ImdbApi.DTOs.Response;

namespace ImdbApi.Interfaces.Services
{
    public interface IAuthService
    {
        public Task<AuthResponseDTO> RegisterAsync(AuthRegisterRequestDTO dto);
        public Task<string?> LoginAsync(AuthLoginRequestDTO dto);
    }
}
