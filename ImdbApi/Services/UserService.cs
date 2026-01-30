using ImdbApi.DTOs.Response;
using ImdbApi.Interfaces;
using ImdbApi.Mappers;

namespace ImdbApi.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;
        private readonly UserMapper _mapper;

        public UserService(IUserRepository repository, UserMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<UserResponse>> GetAllUsers()
        {
            var users = await _repository.GetAllUsersAsync();
            var usersReturn = users.Where(u => u.IsActive.Equals(true)).Select(u => _mapper.ToUserResponse(u));

            return usersReturn;
        }

        public async Task<UserResponse?> GetUserById(int id)
        {
            var user = await _repository.GetUserByIdAsync(id);
            if (user == null) return null;

            return _mapper.ToUserResponse(user);
        }

        public async Task<bool> DeactivateUser(int id)
        {
            var user = await _repository.GetUserByIdAsync(id);
            if (user == null) return false;
            user.IsActive = false;
            await _repository.DeactivateUser(user);

            return true;
        }
    }
}
