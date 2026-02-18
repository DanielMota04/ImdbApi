using Application.DTOs.Request.Auth;
using Application.DTOs.Response.Auth;
using Application.Interfaces;
using Domain.Enums;
using Domain.Errors;
using Domain.Interface.Repositories;
using Domain.Models;

namespace ImdbApiTests.Services
{
    public class AuthServiceTests
    {
        private readonly IJwtService _jwtServiceMock;
        private readonly IUserRepository _userRepositoryMock;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            _jwtServiceMock = Substitute.For<IJwtService>();
            _userRepositoryMock = Substitute.For<IUserRepository>();
            _authService = new AuthService(_jwtServiceMock, _userRepositoryMock);
        }

        // RegisterAsync
        [Fact]
        public async Task RegisterAsync_WhenEmailIsAlreadyRegistered_ReturnFail()
        {
            var email = authRegisterRequestDTO.Email;
            _userRepositoryMock.UserExistsByEmail(email).Returns(true);

            var result = await _authService.RegisterAsync(authRegisterRequestDTO);

            Assert.True(result.IsFailed);
            Assert.Equal("Email already exists", result.Errors.First().Message);

            await _userRepositoryMock
                .DidNotReceive()
                .CreateUser(Arg.Any<User>());
        }

        [Fact]
        public async Task RegisterAsync_WhenEmailIsNotRegistered_ReturnAuthResponse()
        {
            var email = authRegisterRequestDTO.Email;
            _userRepositoryMock.UserExistsByEmail(email).Returns(false);

            var result = await _authService.RegisterAsync(authRegisterRequestDTO);

            Assert.True(result.IsSuccess);
            Assert.Equal(authRegisterRequestDTO.Name, result.Value.Name);

            await _userRepositoryMock
                .Received(1)
                .CreateUser(Arg.Any<User>());
        }

        // LoginAsync
        [Fact]
        public async Task LoginAsync_WhenUserDoesNotExist_ReturnUnauthorized()
        {
            string email = "emailTest@email.com";
            _userRepositoryMock.FindUserByEmail(email).Returns(null as User);

            var result = await _authService.LoginAsync(authLoginRequestDTO);

            Assert.True(result.IsFailed);
            Assert.Equal("Invalid credentials.", result.Errors.First().Message);

            await _userRepositoryMock
                .DidNotReceive()
                .SaveRefreshToken(Arg.Any<RefreshToken>());
        }

        [Fact]
        public async Task LoginAsync_WhenPasswordIsIncorrect_ReturnFail()
        {
            var email = user.Email;
            _userRepositoryMock.FindUserByEmail(email).Returns(user);

            var result = await _authService.LoginAsync(authLoginRequestDTOWithWrongPassword);

            Assert.True(result.IsFailed);
            Assert.Equal("Invalid credentials.", result.Errors.First().Message);

            await _userRepositoryMock
                .DidNotReceive()
                .SaveRefreshToken(Arg.Any<RefreshToken>());
        }

        [Fact]
        public async Task LoginAsync_WhenCredentialsAreValid_ReturnSuccess()
        {
            var email = authLoginRequestDTO.Email;
            var password = authLoginRequestDTO.Password;

            _userRepositoryMock.FindUserByEmail(email).Returns(user);

            _jwtServiceMock.GenerateToken(user).Returns(tokenResponse);

            var result = await _authService.LoginAsync(authLoginRequestDTO);

            Assert.True(result.IsSuccess);
            Assert.Equal("access-token", result.Value.AccessToken);
            await _userRepositoryMock
                .Received(1)
                .SaveRefreshToken(Arg.Any<RefreshToken>());
        }


        private AuthRegisterRequestDTO authRegisterRequestDTO = new()
        {
            Name = "username",
            Email = "user@email.com",
            Password = "123",
            Role = Roles.Admin
        };

        private AuthLoginRequestDTO authLoginRequestDTO = new()
        {
            Email = "user@email.com",
            Password = "123456"
        };

        private AuthLoginRequestDTO authLoginRequestDTOWithWrongPassword = new()
        {
            Email = "user@email.com",
            Password = "000000"
        };

        private User user = new()
        {
            Id = 1,
            Name = "username",
            Email = "user@email.com",
            Password = BCrypt.Net.BCrypt.HashPassword("123456")
        };

        private TokenResponse tokenResponse = new()
        {
            AccessToken = "access-token",
            RefreshToken = "refresh-token"
        };
    }
}