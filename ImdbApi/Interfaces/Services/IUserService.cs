using ImdbApi.DTOs.Pagination;
using ImdbApi.DTOs.Request.User;
using ImdbApi.DTOs.Response.User;
using ImdbApi.Enums;

namespace ImdbApi.Interfaces.Services
{
    public interface IUserService
    {
        public Task<PagedResult<UserResponse>> GetAllUsers(PaginationParams paginationParams, Roles? role);
        public Task<UserResponse?> GetUserById(int id);
        public Task<bool> DeactivateUser(int id);
        public Task<bool> DeactivateMe();
        public Task<UserResponse> UpdateUser(int id, UpdateUserRequestDTO dto);
    }
}
