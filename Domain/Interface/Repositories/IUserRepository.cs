using Domain.Models;

namespace Domain.Interface.Repositories
{
    public interface IUserRepository
    {
        public Task<IEnumerable<User>> GetAllUsersAsync();
        public Task<User?> GetUserByIdAsync(int id);
        public Task<User> CreateUser(User u);
        public Task<User> DeactivateUser(User u);
        public Task<User> UpdateUser(User u);
        public Task<bool> UserExistsByEmail(string email);
        public Task<User?> FindUserByEmail(string email);
    }
}
