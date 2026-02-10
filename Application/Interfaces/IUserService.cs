using Application.DTOs.Pagination;
using Application.DTOs.Request.User;
using Application.DTOs.Response.User;
using Domain.Enums;

namespace Application.Interfaces
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
