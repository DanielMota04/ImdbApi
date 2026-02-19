using Application.DTOs.Request.User;
using Application.DTOs.Response.User;
using Domain.Enums;
using Domain.Models.Pagination;
using FluentResults;
using System.Threading;

namespace Application.Interfaces
{
    public interface IUserService
    {
        public Task<Result<PagedResult<UserResponse>>> GetAllUsers(PaginationParams paginationParams, Roles? role, CancellationToken cancellationToken = default);
        public Task<Result<UserResponse>> GetUserById(int id, CancellationToken cancellationToken = default);
        public Task<Result<bool>> DeactivateUser(int id, CancellationToken cancellationToken = default);
        public Task<Result<bool>> DeactivateMe(int userId, CancellationToken cancellationToken = default);
        public Task<Result<UserResponse>> UpdateUser(int id, UpdateUserRequestDTO dto, int loggedUser, CancellationToken cancellationToken = default);
    }
}
