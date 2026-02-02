using ImdbApi.DTOs.Request;
using ImdbApi.DTOs.Response;
using ImdbApi.Enums;

namespace ImdbApi.Interfaces.Services
{
    public interface IUserService
    {
        public Task<IEnumerable<UserResponse>> GetAllUsers(Roles? role);
        public Task<UserResponse?> GetUserById(int id);
        public Task<bool> DeactivateUser(int id);
        public Task<UserResponse> UpdateUser(int id, UpdateUserRequestDTO dto);
    }
}
