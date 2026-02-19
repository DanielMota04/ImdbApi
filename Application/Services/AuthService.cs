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

        public async Task<Result<AuthResponseDTO>> RegisterAsync(AuthRegisterRequestDTO dto, CancellationToken cancellationToken = default)
        {
            RegisterValidator validator = new();

            string normalizedEmail = dto.Email.Trim().ToLower();

            bool emailAreadyExists = await _userRepository.UserExistsByEmail(normalizedEmail, cancellationToken);
            if (emailAreadyExists)
                return Result.Fail(new ConflictError("Email already exists"));

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            var userEntity = AuthMapper.RegisterToEntity(dto, normalizedEmail, passwordHash);

            validator.ValidateAndThrow(userEntity);

            await _userRepository.CreateUser(userEntity, cancellationToken);

            return Result.Ok(AuthMapper.EntityToResponse(userEntity));
        }

        public async Task<Result<AuthLoginResponseDTO>> LoginAsync(AuthLoginRequestDTO dto, CancellationToken cancellationToken = default)
        {
            var normalizedEmail = dto.Email.Trim().ToLower();

            var user = await _userRepository.FindUserByEmail(normalizedEmail, cancellationToken);

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
            await _userRepository.SaveRefreshToken(refreshTokenEntity, cancellationToken);

            return Result.Ok(new AuthLoginResponseDTO
            {
                Id = user.Id,
                Name = user.Name,
                AccessToken = accessToken,
                RefreshToken = refreshTokenValue
            });
        }

        public async Task<Result<AuthLoginResponseDTO>> RefreshTokenAsync(string token, CancellationToken cancellationToken = default)
        {
            var savedToken = await _userRepository.GetRefreshToken(token, cancellationToken);

            if (savedToken == null || savedToken.IsExpired)
                return Result.Fail(new UnauthorizedError("Refresh token invalid or expired."));

            var user = await _userRepository.GetUserByIdAsync(savedToken.UserId, cancellationToken);

            await _userRepository.DeleteRefreshToken(savedToken, cancellationToken);

            return await GenerateAuthResponse(user!, cancellationToken);
        }

        private async Task<AuthLoginResponseDTO> GenerateAuthResponse(User user, CancellationToken cancellationToken = default)
        {
            var accessToken = _jwtService.GenerateToken(user).AccessToken;
            var refreshTokenValue = _jwtService.GenerateRefreshToken();

            var refreshTokenEntity = new RefreshToken
            {
                Token = refreshTokenValue,
                Expires = DateTime.UtcNow.AddDays(7),
                UserId = user.Id
            };

            await _userRepository.SaveRefreshToken(refreshTokenEntity, cancellationToken);

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
