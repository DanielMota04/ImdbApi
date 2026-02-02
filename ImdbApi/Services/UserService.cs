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

        public async Task<IEnumerable<UserResponse>> GetAllUsers(Roles? role)
        {
            var users = await _userRepository.GetAllUsersAsync();
            var usersReturn = users.Where(u => u.IsActive.Equals(true));

            if (role.HasValue)
            {
                return usersReturn.Where(u => u.Role.Equals(role)).Select(u => _mapper.ToUserResponse(u)).OrderBy(s => s.Name);
            }

            return usersReturn.Select(u => _mapper.ToUserResponse(u)).OrderBy(s => s.Name);
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

            user.Name = dto.Name;
            user.Password = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            await _userRepository.UpdateUser(user);

            return _mapper.ToUserResponse(user);

        }
    }
}
