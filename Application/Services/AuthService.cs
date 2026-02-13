using Application.DTOs.Request.Auth;
using Application.DTOs.Response.Auth;
using Application.Interfaces;
using Application.Mappers;
using Application.Validators;
using Domain.Errors;
using Domain.Interface.Repositories;
using Domain.Models;
using FluentResults;
using FluentValidation;

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

        public async Task<Result<AuthLoginResponseDTO>> LoginAsync(AuthLoginRequestDTO dto)
        {
            var normalizedEmail = dto.Email.Trim().ToLower();

            var user = await _userRepository.FindUserByEmail(normalizedEmail);

            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.Password))
                return Result.Fail(new UnauthorizedError("Invalid credentials."));


            var accessToken = _jwtService.GenerateToken(user).AccessToken;
            var refreshTokenValue = _jwtService.GenerateRefreshToken();

            var refreshTokenEntity = new RefreshToken
            {
                Token = refreshTokenValue,
                Expires = DateTime.UtcNow.AddDays(7),
                UserId = user.Id
            };
            await _userRepository.SaveRefreshToken(refreshTokenEntity);

            return new AuthLoginResponseDTO
            {
                Id = user.Id,
                Name = user.Name,
                AccessToken = accessToken,
                RefreshToken = refreshTokenValue
            };
        }

        public async Task<Result<AuthLoginResponseDTO>> RefreshTokenAsync(string token)
        {
            var savedToken = await _userRepository.GetRefreshToken(token);

            if (savedToken == null || savedToken.IsExpired)
                return Result.Fail(new UnauthorizedError("Refresh token invalid or expired."));

            var user = await _userRepository.GetUserByIdAsync(savedToken.UserId);

            await _userRepository.DeleteRefreshToken(savedToken);

            return await GenerateAuthResponse(user!);
        }

        private async Task<AuthLoginResponseDTO> GenerateAuthResponse(User user)
        {
            var accessToken = _jwtService.GenerateToken(user).AccessToken;
            var refreshTokenValue = _jwtService.GenerateRefreshToken();

            var refreshTokenEntity = new RefreshToken
            {
                Token = refreshTokenValue,
                Expires = DateTime.UtcNow.AddDays(7),
                UserId = user.Id
            };

            await _userRepository.SaveRefreshToken(refreshTokenEntity);

            return new AuthLoginResponseDTO
            {
                Id = user.Id,
                Name = user.Name,
                AccessToken = accessToken,
                RefreshToken = refreshTokenValue
            };
        }
    }
}
