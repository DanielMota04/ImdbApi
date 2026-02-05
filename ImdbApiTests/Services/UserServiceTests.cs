

using ImdbApi.DTOs.Request.User;
using ImdbApi.DTOs.Response.User;
using ImdbApi.Enums;
using ImdbApi.Exceptions;
using ImdbApi.Interfaces.Repositories;
using ImdbApi.Mappers;
using ImdbApi.Models;
using ImdbApi.Services;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace ImdbApiTests.Services
{
    public class UserServiceTests
    {
        private readonly UserMapper _mapper;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;

        private readonly UserService _userService;

        public UserServiceTests()
        {
            _mapper = new UserMapper();
            _userRepositoryMock = new Mock<IUserRepository>();
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();

            _userService = new UserService(_userRepositoryMock.Object, _mapper, _httpContextAccessorMock.Object);
        }

        private void MockUserLogin(string userId)
        {
            var context = new DefaultHttpContext();
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId)
            };

            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            context.User = claimsPrincipal;

            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(context);
        }

        [Fact]
        public async Task DeactivateUser_WhenUserDoesNotExists_ThrowException()
        {
            int userId = 99;

            _userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(userId)).ReturnsAsync((User)null);

            await Assert.ThrowsAsync<ResourceNotFoundException>(() => _userService.DeactivateUser(userId));

            _userRepositoryMock.Verify(x => x.DeactivateUser(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task DeactivateUser_WhenUserExists_ReturnTrue()
        {
            int userId = 1;

            User user = new()
            {
                Id = userId,
                Name = "username",
                Email = "user@email.com",
                Password = "123456",
                Role = Roles.Admin,
                IsActive = false
            };

            _userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(userId)).ReturnsAsync(user);

            var result = await _userService.DeactivateUser(userId);
            Assert.True(result);

            _userRepositoryMock.Verify(x => x.DeactivateUser(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task UpdateUser_WhenUserDoesNotExists_ThrowException()
        {
            int userId = 100;

            UpdateUserRequestDTO dto = new()
            {
                Name = "username",
                Password = "123456"
            };

            _userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(userId)).ReturnsAsync((User)null);

            await Assert.ThrowsAsync<ResourceNotFoundException>(() => _userService.UpdateUser(userId, dto));

            _userRepositoryMock.Verify(x => x.UpdateUser(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task UpdateUser_WhenUserIsNotLogged_ThrowException()
        {
            int userId = 100;
            int loggedUserId = 1;
            MockUserLogin(loggedUserId.ToString());


            UpdateUserRequestDTO dto = new()
            {
                Name = "username",
                Password = "123456"
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

            _userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(userId)).ReturnsAsync(user);

            await Assert.ThrowsAsync<ForbiddenException>(() => _userService.UpdateUser(userId, dto));

            _userRepositoryMock.Verify(x => x.UpdateUser(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task UpdateUser_WhenUserExistsAndIsLogged_ReturnsUserResponse()
        {
            int userId = 1;
            MockUserLogin(userId.ToString());


            UpdateUserRequestDTO dto = new()
            {
                Name = "username",
                Password = "123456"
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

            _userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(userId)).ReturnsAsync(user);

            var result = await _userService.UpdateUser(userId ,dto);

            Assert.NotNull(result);
            Assert.Equal(result.Role, user.Role);
            Assert.Equal(result.Name, user.Name);

            _userRepositoryMock.Verify(x => x.UpdateUser(It.IsAny<User>()), Times.Once);
        }
    }
}
