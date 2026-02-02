using ImdbApi.DTOs.Response;
using ImdbApi.Models;

namespace ImdbApi.Interfaces.Services
{
    public interface IUserService
    {
        public Task<IEnumerable<UserResponse>> GetAllUsers(Roles? role);
        public Task<UserResponse?> GetUserById(int id);
        public Task<bool> DeactivateUser(int id);
        public Task<double?> Vote(int movieId, double vote);
    }
}
