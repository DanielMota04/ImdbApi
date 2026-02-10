using Domain.Models;

namespace Application.Interfaces
{
    public interface IJwtService
    {
        public string GenerateToken(User user);
    }
}
