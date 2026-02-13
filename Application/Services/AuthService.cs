using Application.DTOs.Request.Auth;
using Application.DTOs.Response.Auth;
using Application.Interfaces;
using Application.Mappers;
using Application.Validators;
using Domain.Interface.Repositories;
using FluentValidation;
using FluentResults;
using Domain.Errors;

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

        public async Task<Result<AuthResponseDTO>> RegisterAsync(AuthRegisterRequestDTO dto)
        {
            RegisterValidator validator = new();

            string normalizedEmail = dto.Email.Trim().ToLower();

            bool emailAreadyExists = await _userRepository.UserExistsByEmail(normalizedEmail);
            if (emailAreadyExists)
                return Result.Fail(new ConflictError("Email already exists"));

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            var userEntity = AuthMapper.RegisterToEntity(dto, normalizedEmail, passwordHash);

            validator.ValidateAndThrow(userEntity);

            await _userRepository.CreateUser(userEntity);

            return Result.Ok(AuthMapper.EntityToResponse(userEntity));
        }

        public async Task<Result<string>> LoginAsync(AuthLoginRequestDTO dto)
        {
            var normalizedEmail = dto.Email.Trim().ToLower();

            var user = await _userRepository.FindUserByEmail(normalizedEmail);

            if (user == null)
                return Result.Fail(new UnauthorizedError("Invalid credentials."));

            var passwordIsValid = BCrypt.Net.BCrypt.Verify(dto.Password, user.Password);

            if (!passwordIsValid)
                return Result.Fail(new UnauthorizedError("Invalid credentials."));

            return Result.Ok(_jwtService.GenerateToken(user));
        }
    }
}
