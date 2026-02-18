using Application.DTOs.Request.User;
using Application.DTOs.Response.User;
using Domain.Enums;
using Domain.Models.Pagination;
using FluentResults;

namespace Application.Interfaces
{
    public interface IUserService
    {
        public Task<Result<PagedResult<UserResponse>>> GetAllUsers(PaginationParams paginationParams, Roles? role);
        public Task<Result<UserResponse>> GetUserById(int id);
        public Task<Result<bool>> DeactivateUser(int id);
        public Task<Result<bool>> DeactivateMe(int userId);
        public Task<Result<UserResponse>> UpdateUser(int id, UpdateUserRequestDTO dto, int loggedUser);
    }
}
