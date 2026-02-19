using Application.DTOs.Request.Auth;
using Application.DTOs.Response.Auth;
using FluentResults;

namespace Application.Interfaces
{
    public interface IAuthService
    {
        public Task<Result<AuthResponseDTO>> RegisterAsync(AuthRegisterRequestDTO dto, CancellationToken cancellationToken = default);
        public Task<Result<AuthLoginResponseDTO>> LoginAsync(AuthLoginRequestDTO dto, CancellationToken cancellationToken = default);
        public Task<Result<AuthLoginResponseDTO>> RefreshTokenAsync(string token, CancellationToken cancellationToken = default);
    }
}
