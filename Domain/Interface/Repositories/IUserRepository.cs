using Domain.Enums;
using Domain.Models;
using Domain.Models.Pagination;

namespace Domain.Interface.Repositories
{
    public interface IUserRepository
    {
        public Task<PagedResult<User>> GetAllUsersAsync(PaginationParams paginationParams, Roles? role);
        public Task<User?> GetUserByIdAsync(int id);
        public Task<User> CreateUser(User u);
        public Task<User> DeactivateUser(User u);
        public Task<User> UpdateUser(User u);
        public Task<bool> UserExistsByEmail(string email);
        public Task<User?> FindUserByEmail(string email);
    }
}
