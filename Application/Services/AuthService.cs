using FluentValidation;
using ImdbApi.DTOs.Request.Auth;
using ImdbApi.DTOs.Response.Auth;
using ImdbApi.Exceptions;
using ImdbApi.Interfaces.Repositories;
using ImdbApi.Interfaces.Services;
using ImdbApi.Mappers;
using ImdbApi.Validators;

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
            RegisterValidator validator = new RegisterValidator();

            string normalizedEmail = dto.Email.Trim().ToLower();

            if (await _userRepository.UserExistsByEmail(normalizedEmail)) throw new ConflictException("Email already in use.");

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            var userEntity = _mapper.RegisterToEntity(dto, normalizedEmail, passwordHash);

            validator.ValidateAndThrow(userEntity);

            await _userRepository.CreateUser(userEntity);

            return _mapper.EntityToResponse(userEntity);
        }

        public async Task<string?> LoginAsync(AuthLoginRequestDTO dto)
        {
            var normalizedEmail = dto.Email.Trim().ToLower();

            var user = await _userRepository.FindUserByEmail(normalizedEmail);
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.Password)) throw new UnauthorizedAccessException("Invalid credentials.");

            return _jwtService.GenerateToken(user);
        }
    }
}
