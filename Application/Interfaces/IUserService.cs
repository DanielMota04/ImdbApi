using Application.DTOs.Request.User;
using Application.DTOs.Response.User;
using Domain.Enums;
using Domain.Models.Pagination;

namespace Application.Interfaces
{
    public interface IUserService
    {
        public Task<PagedResult<UserResponse>> GetAllUsers(PaginationParams paginationParams, Roles? role);
        public Task<UserResponse?> GetUserById(int id);
        public Task<bool> DeactivateUser(int id);
        public Task<bool> DeactivateMe(int userId);
        public Task<UserResponse> UpdateUser(int id, UpdateUserRequestDTO dto, int loggedUser);
    }
}
