using ImdbApi.Data;
using ImdbApi.Interfaces;
using ImdbApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ImdbApi.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            var users = await _context.Users.ToListAsync();
            return users;
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            return user;
        }

        public async Task<User> DeactivateUser(User u)
        {
            var user = await GetUserByIdAsync(u.Id);
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<User> CreateUser(User u)
        {
            _context.Users.Add(u);
            await _context.SaveChangesAsync();

            return u;
        }

        public async Task<bool> UserExistsByEmail(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }

        public async Task<User?> FindUserByEmail(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            return user;
        }
    }
}
