using Domain.Enums;
using Domain.Models;
using Domain.Models.Pagination;

namespace Domain.Interface.Repositories
{
    public interface IUserRepository
    {
        public Task<PagedResult<User>> GetAllUsersAsync(PaginationParams paginationParams, Roles? role, CancellationToken cancellationToken = default);
        public Task<User?> GetUserByIdAsync(int id, CancellationToken cancellationToken = default);
        public Task<User> CreateUser(User user, CancellationToken cancellationToken = default);
        public Task<User> DeactivateUser(User user, CancellationToken cancellationToken = default);
        public Task<User> UpdateUser(User user, CancellationToken cancellationToken = default);
        public Task<bool> UserExistsByEmail(string email, CancellationToken cancellationToken = default);
        public Task<User?> FindUserByEmail(string email, CancellationToken cancellationToken = default);
        public Task SaveRefreshToken(RefreshToken token, CancellationToken cancellationToken = default);
        public Task<RefreshToken?> GetRefreshToken(string token, CancellationToken cancellationToken = default);
        public Task DeleteRefreshToken(RefreshToken token, CancellationToken cancellationToken = default);
    }
}
