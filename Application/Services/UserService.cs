using Application.DTOs.Pagination;
using Application.DTOs.Request.User;
using Application.DTOs.Response.User;
using Application.Interfaces;
using Application.Mappers;
using Domain.Enums;
using Domain.Exceptions;
using Domain.Interface.Repositories;

namespace Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<PagedResult<UserResponse>> GetAllUsers(PaginationParams paginationParams, Roles? role)
        {
            var allUsers = await _userRepository.GetAllUsersAsync();
            var query = allUsers.AsQueryable();

            query = query.Where(u => u.IsActive);

            if (role.HasValue)
            {
                query = query.Where(u => u.Role.Equals(role));
            }

            var totalItems = query.Count();

            var pagedUsers = query.OrderBy(u => u.Name)
                .Skip((paginationParams.PageNumber - 1) * paginationParams.PageSize)
                .Take(paginationParams.PageSize)
                .ToList();

            var mappedUsers = pagedUsers.Select(u => UserMapper.ToUserResponse(u));

            return new PagedResult<UserResponse>
            {
                Items = mappedUsers,
                TotalItems = totalItems,
                PageNumber = paginationParams.PageNumber,
                PageSize = paginationParams.PageSize
            };
        }

        public async Task<UserResponse?> GetUserById(int id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null)
                throw new ResourceNotFoundException($"User not found by id {id}.");

            return UserMapper.ToUserResponse(user);
        }

        public async Task<bool> DeactivateUser(int id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null)
                throw new ResourceNotFoundException($"User not found by id {id}.");
            user.IsActive = false;
            await _userRepository.DeactivateUser(user);

            return true;
        }

        public async Task<bool> DeactivateMe(int userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null) 
                throw new ResourceNotFoundException($"User not found by id {userId}.");
            user.IsActive = false;
            await _userRepository.DeactivateUser(user);

            return true;
        }

        public async Task<UserResponse> UpdateUser(int id, UpdateUserRequestDTO dto, int loggedUser)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null) 
                throw new ResourceNotFoundException($"User not found by id {id}.");

            if (loggedUser != id) 
                throw new ForbiddenException("You cannot update other users data.");

            if (dto.Name != "" && dto.Name != null)
                user.Name = dto.Name;

            if (dto.Password != "" && dto.Password != null)
                user.Password = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            await _userRepository.UpdateUser(user);

            return UserMapper.ToUserResponse(user);

        }
    }
}
