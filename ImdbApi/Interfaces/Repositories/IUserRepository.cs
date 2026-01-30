using ImdbApi.Models;

namespace ImdbApi.Interfaces.Repositories
{
    public interface IUserRepository
    {
        public Task<IEnumerable<User>> GetAllUsersAsync();
        public Task<User?> GetUserByIdAsync(int id);
        public Task<User> DeactivateUser(User u);
        public Task<User> CreateUser(User u);
        public Task<bool> UserExistsByEmail(string email);
        public Task<User?> FindUserByEmail(string email);
    }
}
