using Application.DTOs.Request.Auth;
using Application.Interfaces;
using Domain.Enums;
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
        // RegisterAsync
        // RegisterAsync

        // LoginAsync
        // LoginAsync
        // LoginAsync
        // LoginAsync

        private AuthRegisterRequestDTO authRegisterRequestDTO = new()
        {
            Name = "username",
            Email = "user@email.com",
            Password = "123",
            Role = Roles.Admin
        };

        private User user = new()
        {
            Id = 1,
            Name = "username",
            Email = "user@email.com",
            Password = "123456",
            Role = Roles.Admin,
            IsActive = false
        };

        private AuthLoginRequestDTO authLoginRequestDTO = new()
        {
            Email = "user@email.com",
            Password = "123456"
        };
    }
}
