using Application.DTOs.Request.Auth;
using Application.DTOs.Response.Auth;
using FluentResults;

namespace Application.Interfaces
{
    public interface IAuthService
    {
        public Task<Result<AuthResponseDTO>> RegisterAsync(AuthRegisterRequestDTO dto);
        public Task<Result<AuthLoginResponseDTO>> LoginAsync(AuthLoginRequestDTO dto);
        public Task<Result<AuthLoginResponseDTO>> RefreshTokenAsync(string token);
    }
}
