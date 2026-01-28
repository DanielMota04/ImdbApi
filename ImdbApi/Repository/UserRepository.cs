using ImdbApi.Data;
using ImdbApi.DTOs.Request;
using ImdbApi.DTOs.Response;
using ImdbApi.Mappers;
using ImdbApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ImdbApi.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;
        private readonly UserMapper _mapper;
        public UserRepository(AppDbContext context, UserMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }


        public async Task<UserResponse> GetUserById(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return null;
            return _mapper.ToUserResponseDTO(user);
        }

        public async Task<List<UserResponse>> GetUsers()
        {
            var users = await _context.Users.ToListAsync();
            List<UserResponse> response = new List<UserResponse>();
            foreach (var user in users)
            {
                response.Add(_mapper.ToUserResponseDTO(user));
            }
            return response;
        }

        public async Task<AuthResponseDTO> Register(AuthRegisterRequestDTO request)
        {
            int id = 0; //temporario
            User user = _mapper.RegisterToEntity(request, id);
            _context.Users.Add(user);
            _context.SaveChanges();

            return _mapper.ToAuthResponseDTO(user);

        }

        public Task<AuthLoginResponseDTO> Login(AuthLoginRequestDTO request)
        {
            throw new NotImplementedException();
        }

    }
}
