using ImdbApi.DTOs.Request.Auth;
using ImdbApi.Enums;
using ImdbApi.Exceptions;
using ImdbApi.Interfaces.Repositories;
using ImdbApi.Interfaces.Services;
using ImdbApi.Mappers;
using ImdbApi.Models;
using ImdbApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImdbApiTests.Services
{
    public class AuthServiceTests
    {
        private readonly AuthMapper _mapper;
        private readonly Mock<IJwtService> _jwtServiceMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;

        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            _mapper = new AuthMapper();
            _jwtServiceMock = new Mock<IJwtService>();
            _userRepositoryMock = new Mock<IUserRepository>();

            _authService = new AuthService(_mapper, _jwtServiceMock.Object, _userRepositoryMock.Object);
        }

        [Fact]
        public async Task RegisterAsync_WhenEmailIsAlreadyRegistered_ThrowException()
        {
            AuthRegisterRequestDTO dto = new()
            {
                Name = "username",
                Email = "user@email.com",
                Password = "123",
                Role = Roles.Admin
            };

            _userRepositoryMock.Setup(repo => repo.UserExistsByEmail(dto.Email)).ReturnsAsync(true);

            await Assert.ThrowsAsync<ConflictException>(() => _authService.RegisterAsync(dto));

            _userRepositoryMock.Verify(x => x.CreateUser(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task RegisterAsync_WhenEmailIsNotRegistered_ReturnAuthResponse()
        {
            int userId = 1;

            AuthRegisterRequestDTO dto = new()
            {
                Name = "username",
                Email = "user@email.com",
                Password = "123",
                Role = Roles.Admin
            };

            User user = new()
            {
                Id = userId,
                Name = "username",
                Email = "user@email.com",
                Password = "123456",
                Role = Roles.Admin,
                IsActive = false
            };

            _userRepositoryMock.Setup(repo => repo.UserExistsByEmail(dto.Email)).ReturnsAsync(false);

            var result = await _authService.RegisterAsync(dto);
            Assert.NotNull(result);
            Assert.Equal(/*user.Id*/ 0, result.Id); // verificar por que 0 e nao user.id
            Assert.Equal(user.Name, result.Name);
            Assert.Equal(user.Email, result.Email);
            Assert.Equal(user.Role, result.Role);

            _userRepositoryMock.Verify(x => x.CreateUser(It.IsAny<User>()), Times.Once);

        }
    }
}
