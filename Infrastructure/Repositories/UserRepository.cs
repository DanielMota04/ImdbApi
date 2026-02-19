using Domain.Enums;
using Domain.Interface.Repositories;
using Domain.Models;
using Domain.Models.Pagination;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<User>> GetAllUsersAsync(PaginationParams paginationParams, Roles? role, CancellationToken cancellationToken)
        {
            var query = _context.Users.AsQueryable().Where(u => u.IsActive);

            if (role.HasValue)
                query = query.Where(u => u.Role == role.Value);

            var totalItems = await query.CountAsync();

            var items = await query
                .OrderBy(u => u.Name)
                .Skip((paginationParams.PageNumber - 1) * paginationParams.PageSize)
                .Take(paginationParams.PageSize)
                .ToListAsync();

            return new PagedResult<User>
            {
                Items = items,
                TotalItems = totalItems,
                PageNumber = paginationParams.PageNumber,
                PageSize = paginationParams.PageSize
            };
        }

        public async Task<User?> GetUserByIdAsync(int id, CancellationToken cancellationToken)
        {
            var user = await _context.Users.FindAsync(id);
            return user;
        }
        
        public async Task<User> DeactivateUser(User user, CancellationToken cancellationToken)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return user;
        }
        
        public async Task<User> CreateUser(User user, CancellationToken cancellationToken)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return user;
        }
        
        public async Task<User> UpdateUser(User user, CancellationToken cancellationToken)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return user;
        }
        
        public async Task<bool> UserExistsByEmail(string email, CancellationToken cancellationToken)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }

        public async Task<User?> FindUserByEmail(string email, CancellationToken cancellationToken)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            return user;
        }

        public async Task SaveRefreshToken(RefreshToken token, CancellationToken cancellationToken)
        {
            _context.RefreshTokens.Add(token);
            await _context.SaveChangesAsync();
        }

        public Task<RefreshToken?> GetRefreshToken(string token, CancellationToken cancellationToken)
        {
            return _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == token);
        }

        public Task DeleteRefreshToken(RefreshToken token, CancellationToken cancellationToken)
        {
            _context.RefreshTokens.Remove(token);
            return _context.SaveChangesAsync();
        }
    }
}
