using ImdbApi.Models;

namespace ImdbApi.Interfaces.Services
{
    public interface IJwtService
    {
        public string GenerateToken(User user);
    }
}
