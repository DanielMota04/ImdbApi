using ImdbApi.Data;
using ImdbApi.DTOs.Response;
using ImdbApi.Interfaces;
using ImdbApi.Mappers;
using ImdbApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ImdbApi.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;
        private readonly UserMapper _mapper;

        public UserService(AppDbContext context, UserMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<UserResponse>> GetAllUsers()
        {
            var users = await _context.Users.ToListAsync();
            var usersReturn = users.Where(u => u.IsActive.Equals(true)).Select(u => _mapper.ToUserResponse(u));

            return usersReturn;
        }

        public async Task<UserResponse?> GetUserById(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return null;

            return _mapper.ToUserResponse(user);
        }

        public async Task<bool> DeactivateUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return false;
            user.IsActive = false;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
