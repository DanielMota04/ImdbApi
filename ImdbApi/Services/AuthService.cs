using ImdbApi.Data;
using ImdbApi.DTOs.Request;
using ImdbApi.DTOs.Response;
using ImdbApi.Mappers;
using ImdbApi.Repository;
using Microsoft.EntityFrameworkCore;

namespace ImdbApi.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly AuthMapper _mapper;
        private readonly IJwtService _jwtService;
        public AuthService(AppDbContext context, AuthMapper mapper, IJwtService jwtService) 
        {
            _context = context;
            _mapper = mapper;
            _jwtService = jwtService;
        }

        public async Task<AuthResponseDTO> RegisterAsync(AuthRegisterRequestDTO dto)
        {
            string normalizedEmail = dto.Email.Trim().ToLower();

            if (await _context.Users.AnyAsync(u => u.Email == normalizedEmail))
            {
                return null;
            }

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            var userEntity = _mapper.RegisterToEntity(dto, normalizedEmail, passwordHash);

            _context.Users.Add(userEntity);
            await _context.SaveChangesAsync();

            return _mapper.EntityToResponse(userEntity);
        }

        public async Task<string?> LoginAsync(AuthLoginRequestDTO dto)
        {
            var normalizedEmail = dto.Email.Trim().ToLower();

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == normalizedEmail);
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.Password)) return null;

            return _jwtService.GenerateToken(user);
        }
    }
}
