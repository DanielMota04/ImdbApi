using ImdbApi.DTOs.Pagination;
using ImdbApi.DTOs.Request.User;
using ImdbApi.DTOs.Response.User;
using ImdbApi.Enums;
using ImdbApi.Exceptions;
using ImdbApi.Interfaces.Repositories;
using ImdbApi.Interfaces.Services;
using ImdbApi.Mappers;
using System.Security.Claims;

namespace ImdbApi.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly UserMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UserService(IUserRepository userRepository, UserMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
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

            var mappedUsers = pagedUsers.Select(u => _mapper.ToUserResponse(u));

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
            if (user == null) throw new ResourceNotFoundException($"User not found by id {id}.");

            return _mapper.ToUserResponse(user);
        }

        public async Task<bool> DeactivateUser(int id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null) throw new ResourceNotFoundException($"User not found by id {id}.");
            user.IsActive = false;
            await _userRepository.DeactivateUser(user);

            return true;
        }

        public async Task<UserResponse> UpdateUser(int id, UpdateUserRequestDTO dto)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null) throw new ResourceNotFoundException($"User not found by id {id}.");
            var userId = int.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            if (userId != id) throw new ForbiddenException("You cannot update other users data.");

            if (dto.Name != "")
            {
                user.Name = dto.Name;
            }
            if (dto.Password != "")
            {
                user.Password = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            }

            await _userRepository.UpdateUser(user);

            return _mapper.ToUserResponse(user);

        }
    }
}
