using Application.DTOs.Request.Auth;
using Application.DTOs.Response.Auth;
using Application.Interfaces;
using Application.Mappers;
using Application.Validators;
using Domain.Interface.Repositories;
using FluentValidation;
using Domain.Exceptions;

namespace Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IJwtService _jwtService;
        private readonly IUserRepository _userRepository;
        public AuthService(IJwtService jwtService, IUserRepository userRepository)
        {
            _jwtService = jwtService;
            _userRepository = userRepository;
        }

        public async Task<AuthResponseDTO> RegisterAsync(AuthRegisterRequestDTO dto)
        {
            RegisterValidator validator = new();

            string normalizedEmail = dto.Email.Trim().ToLower();

            bool emailAreadyExists = await _userRepository.UserExistsByEmail(normalizedEmail);
            if (emailAreadyExists) 
                throw new ConflictException("Email already in use.");

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            var userEntity = AuthMapper.RegisterToEntity(dto, normalizedEmail, passwordHash);

            validator.ValidateAndThrow(userEntity);

            await _userRepository.CreateUser(userEntity);

            return AuthMapper.EntityToResponse(userEntity);
        }

        public async Task<string?> LoginAsync(AuthLoginRequestDTO dto)
        {
            var normalizedEmail = dto.Email.Trim().ToLower();

            var user = await _userRepository.FindUserByEmail(normalizedEmail);

            var passwordIsValid = BCrypt.Net.BCrypt.Verify(dto.Password, user.Password);

            if (user == null || !passwordIsValid)
                throw new UnauthorizedAccessException("Invalid credentials.");

            return _jwtService.GenerateToken(user);
        }
    }
}
