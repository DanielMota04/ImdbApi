using ImdbApi.Models;

namespace ImdbApi.Services
{
    public interface IJwtService
    {
        public string GenerateToken(User user);
    }
}
