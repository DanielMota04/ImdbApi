using Application.DTOs.Request.Auth;
using Application.DTOs.Response.Auth;
using FluentResults;

namespace Application.Interfaces
{
    public interface IAuthService
    {
        public Task<AuthResponseDTO> RegisterAsync(AuthRegisterRequestDTO dto);
        public Task<string?> LoginAsync(AuthLoginRequestDTO dto);
    }
}
