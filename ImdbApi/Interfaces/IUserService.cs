using ImdbApi.DTOs.Response;
using ImdbApi.Models;

namespace ImdbApi.Interfaces
{
    public interface IUserService
    {
        public Task<IEnumerable<UserResponse>> GetAllUsers();
        public Task<UserResponse?> GetUserById(int id);
        public Task<bool> DeactivateUser(int id);
    }
}
