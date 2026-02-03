using ImdbApi.DTOs.Pagination;
using ImdbApi.DTOs.Request.User;
using ImdbApi.DTOs.Response.User;
using ImdbApi.Enums;
using ImdbApi.Interfaces.Repositories;
using ImdbApi.Interfaces.Services;
using ImdbApi.Mappers;

namespace ImdbApi.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly UserMapper _mapper;
        public UserService(IUserRepository userRepository, UserMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
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
            if (user == null) return null;

            return _mapper.ToUserResponse(user);
        }

        public async Task<bool> DeactivateUser(int id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null) return false;
            user.IsActive = false;
            await _userRepository.DeactivateUser(user);

            return true;
        }

        public async Task<UserResponse> UpdateUser(int id, UpdateUserRequestDTO dto)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null) return null;

            user.Name = dto.Name != "" ? dto.Name : user.Name;
            user.Password = dto.Password != "" ? BCrypt.Net.BCrypt.HashPassword(dto.Password) : user.Password;

            await _userRepository.UpdateUser(user);

            return _mapper.ToUserResponse(user);

        }
    }
}
