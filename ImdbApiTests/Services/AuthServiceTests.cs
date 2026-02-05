using ImdbApi.DTOs.Request.Auth;
using ImdbApi.Enums;
using ImdbApi.Exceptions;
using ImdbApi.Interfaces.Repositories;
using ImdbApi.Interfaces.Services;
using ImdbApi.Mappers;
using ImdbApi.Models;
using ImdbApi.Services;

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
            Assert.Equal(0, result.Id);
            Assert.Equal(user.Name, result.Name);
            Assert.Equal(user.Email, result.Email);
            Assert.Equal(user.Role, result.Role);

            _userRepositoryMock.Verify(x => x.CreateUser(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task LoginAsync_WhenUserNotFound_ThrowException()
        {
            AuthLoginRequestDTO dto = new()
            {
                Email = "user@email.com",
                Password = "123456"
            };

            _userRepositoryMock.Setup(repo => repo.FindUserByEmail(dto.Email)).ReturnsAsync((User)null);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _authService.LoginAsync(dto));

            _jwtServiceMock.Verify(x => x.GenerateToken(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task LoginAsync_WhenPasswordIsIncorrect_ThrowException()
        {
            int userId = 1;

            AuthLoginRequestDTO dto = new()
            {
                Email = "user@email.com",
                Password = "123456"
            };

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword("password");

            User user = new()
            {
                Id = userId,
                Name = "username",
                Email = "user@email.com",
                Password = hashedPassword,
                Role = Roles.Admin,
                IsActive = false
            };

            _userRepositoryMock.Setup(repo => repo.FindUserByEmail(dto.Email)).ReturnsAsync(user);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _authService.LoginAsync(dto));

            _jwtServiceMock.Verify(x => x.GenerateToken(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task LoginAsync_WhenCredentialsAreValid_ReturnToken()
        {
            int userId = 1;
            string password = "123456";

            AuthLoginRequestDTO dto = new()
            {
                Email = "user@email.com",
                Password = password
            };

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

            string token = "returnToken";

            User user = new()
            {
                Id = userId,
                Name = "username",
                Email = "user@email.com",
                Password = hashedPassword,
                Role = Roles.Admin,
                IsActive = true
            };

            _userRepositoryMock.Setup(repo => repo.FindUserByEmail(dto.Email)).ReturnsAsync(user);
            _jwtServiceMock.Setup(service => service.GenerateToken(user)).Returns(token);

            var result = await _authService.LoginAsync(dto);

            Assert.NotNull(result);
            Assert.Equal(token, result);

            _jwtServiceMock.Verify(x => x.GenerateToken(It.IsAny<User>()), Times.Once);
        }
    }
}
