using ImdbApi.Data;
using ImdbApi.DTOs.Request.Auth;
using ImdbApi.DTOs.Response.Auth;
using ImdbApi.Interfaces.Repositories;
using ImdbApi.Interfaces.Services;
using ImdbApi.Mappers;

namespace ImdbApi.Services
{
    public class AuthService : IAuthService
    {
        private readonly AuthMapper _mapper;
        private readonly IJwtService _jwtService;
        private readonly IUserRepository _userRepository;
        public AuthService(AuthMapper mapper, IJwtService jwtService, IUserRepository userRepository)
        {
            _mapper = mapper;
            _jwtService = jwtService;
            _userRepository = userRepository;
        }

        public async Task<AuthResponseDTO> RegisterAsync(AuthRegisterRequestDTO dto)
        {
            string normalizedEmail = dto.Email.Trim().ToLower();

            if (await _userRepository.UserExistsByEmail(normalizedEmail))
            {
                return null;
            }

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            var userEntity = _mapper.RegisterToEntity(dto, normalizedEmail, passwordHash);

            await _userRepository.CreateUser(userEntity);

            return _mapper.EntityToResponse(userEntity);
        }

        public async Task<string?> LoginAsync(AuthLoginRequestDTO dto)
        {
            var normalizedEmail = dto.Email.Trim().ToLower();

            var user = await _userRepository.FindUserByEmail(normalizedEmail);
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.Password)) return null;

            return _jwtService.GenerateToken(user);
        }
    }
}
