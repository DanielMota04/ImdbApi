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

        public async Task<Result<PagedResult<UserResponse>>> GetAllUsers(PaginationParams paginationParams, Roles? role)
        {
            var pagedUsers = await _userRepository.GetAllUsersAsync(paginationParams, role);

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

        public async Task<Result<UserResponse>> GetUserById(int id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null)
                return Result.Fail(new NotFoundError($"User not found by id {id}."));

            return Result.Ok(UserMapper.ToUserResponse(user));
        }

        public async Task<Result<bool>> DeactivateUser(int id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null)
                return Result.Fail(new NotFoundError($"User not found by id {id}."));
            
            user.IsActive = false;
            await _userRepository.DeactivateUser(user);

            return Result.Ok(true);
        }

        public async Task<Result<bool>> DeactivateMe(int userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
                return Result.Fail(new NotFoundError($"User not found by id {userId}."));

            user.IsActive = false;
            await _userRepository.DeactivateUser(user);

            return Result.Ok(true);
        }

        public async Task<Result<UserResponse>> UpdateUser(int id, UpdateUserRequestDTO dto, int loggedUser)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null)
                return Result.Fail(new NotFoundError($"User not found by id {id}."));

            if (loggedUser != id)
                return Result.Fail(new ForbiddenError("You cannot update other users data."));

            if (dto.Name != "" && dto.Name != null)
                user.Name = dto.Name;

            if (dto.Password != "" && dto.Password != null)
                user.Password = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            await _userRepository.UpdateUser(user);

            return Result.Ok(UserMapper.ToUserResponse(user));

        }
    }
}
