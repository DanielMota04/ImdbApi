using Application.DTOs.Response.Auth;
using Domain.Models;

namespace Application.Interfaces
{
    public interface IJwtService
    {
        public TokenResponse GenerateToken(User user);
        public string GenerateRefreshToken();
    }
}
