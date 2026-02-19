using Application.DTOs.Request.User;
using Application.DTOs.Response.User;
using Application.Interfaces;
using Application.Mappers;
using Domain.Enums;
using Domain.Errors;
using Domain.Interface.Repositories;
using Domain.Models.Pagination;
using FluentResults;

namespace Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<Result<PagedResult<UserResponse>>> GetAllUsers(PaginationParams paginationParams, Roles? role, CancellationToken cancellationToken = default)
        {
            var pagedUsers = await _userRepository.GetAllUsersAsync(paginationParams, role, cancellationToken);

            var mappedItems = pagedUsers.Items?.Select(u => UserMapper.ToUserResponse(u)).ToList() ?? new List<UserResponse>();

            var result = new PagedResult<UserResponse>
            {
                Items = mappedItems,
                TotalItems = pagedUsers.TotalItems,
                PageNumber = pagedUsers.PageNumber,
                PageSize = pagedUsers.PageSize
            };

            return Result.Ok(result);
        }

        public async Task<Result<UserResponse>> GetUserById(int id, CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.GetUserByIdAsync(id, cancellationToken);
            if (user == null)
                return Result.Fail(new NotFoundError($"User not found by id {id}."));

            return Result.Ok(UserMapper.ToUserResponse(user));
        }

        public async Task<Result<bool>> DeactivateUser(int id, CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.GetUserByIdAsync(id, cancellationToken);
            if (user == null)
                return Result.Fail(new NotFoundError($"User not found by id {id}."));
            
            user.IsActive = false;
            await _userRepository.DeactivateUser(user, cancellationToken);

            return Result.Ok(true);
        }

        public async Task<Result<bool>> DeactivateMe(int userId, CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.GetUserByIdAsync(userId, cancellationToken);
            if (user == null)
                return Result.Fail(new NotFoundError($"User not found by id {userId}."));

            user.IsActive = false;
            await _userRepository.DeactivateUser(user, cancellationToken);

            return Result.Ok(true);
        }

        public async Task<Result<UserResponse>> UpdateUser(int id, UpdateUserRequestDTO dto, int loggedUser, CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.GetUserByIdAsync(id, cancellationToken);
            if (user == null)
                return Result.Fail(new NotFoundError($"User not found by id {id}."));

            if (loggedUser != id)
                return Result.Fail(new ForbiddenError("You cannot update other users data."));

            if (!string.IsNullOrWhiteSpace(dto.Name))
                user.Name = dto.Name;

            if (!string.IsNullOrWhiteSpace(dto.Password))
                user.Password = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            await _userRepository.UpdateUser(user, cancellationToken);

            return Result.Ok(UserMapper.ToUserResponse(user));

        }
    }
}
