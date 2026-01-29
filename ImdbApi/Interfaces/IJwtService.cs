using ImdbApi.Models;

namespace ImdbApi.Interfaces
{
    public interface IJwtService
    {
        public string GenerateToken(User user);
    }
}
