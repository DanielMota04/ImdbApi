using Application.DTOs.Request.User;
using Application.DTOs.Response.User;
using Domain.Interface.Repositories;
using Domain.Models;

namespace ImdbApiTests.Services
{
    public class UserServiceTests
    {
        private readonly IUserRepository _userRepositoryMock;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _userRepositoryMock = Substitute.For<IUserRepository>();
            _userService = new UserService(_userRepositoryMock);
        }

        // GetUserById
        [Fact]
        public async Task GetUserById_WhenUserExists_ReturnSuccess()
        {
            int userId = 1;
            var user = new User { Id = userId, Name = "username", Email = "user@email.com", Password = "password123" };
            var userResponse = new UserResponse { Id = userId, Name = "username", Role = Domain.Enums.Roles.User };


            _userRepositoryMock.GetUserByIdAsync(userId).Returns(user);

            var result = await _userService.GetUserById(userId);

            Assert.True(result.IsSuccess);
            Assert.Equal(userResponse.Id, result.Value.Id);
            Assert.Equal(userResponse.Name, result.Value.Name);
        }

        [Fact]
        public async Task GetUserById_WhenUserDoesNotExists_ReturnFail()
        {
            int userId = 1;
            var user = new User { Id = userId, Name = "username", Email = "user@email.com", Password = "password123" };
            var userResponse = new UserResponse { Id = userId, Name = "username", Role = Domain.Enums.Roles.User };


            _userRepositoryMock.GetUserByIdAsync(userId).Returns((User)null);

            var result = await _userService.GetUserById(userId);

            Assert.True(result.IsFailed);
            Assert.Equal($"User not found by id {userId}.", result.Errors.First().Message);
        }

        // DeactivateUser
        [Fact]
        public async Task DeactivateUser_WhenUserDoesNotExists_ReturnFail()
        {
            int userId = 99;
            _userRepositoryMock.GetUserByIdAsync(userId).Returns((User)null);

            var result = await _userService.DeactivateUser(userId);

            Assert.True(result.IsFailed);
            Assert.Equal($"User not found by id {userId}.", result.Errors.First().Message);

            await _userRepositoryMock.DidNotReceive().DeactivateUser(Arg.Any<User>());
        }

        [Fact]
        public async Task DeactivateUser_WhenUserExists_ReturnSuccess()
        {
            int userId = 1;
            var user = new User { Id = userId, Name = "username", Email = "user@email.com", Password = "password123" };

            _userRepositoryMock.GetUserByIdAsync(userId).Returns(user);

            var result = await _userService.DeactivateUser(userId);

            Assert.True(result.IsSuccess);
        }

        // DeactivateMe
        [Fact]
        public async Task DeactivateMe_WhenUserExists_ReturnSuccess()
        {
            int userId = 1;
            var user = new User { Id = userId, Name = "username", Email = "user@email.com", Password = "password123" };

            _userRepositoryMock.GetUserByIdAsync(userId).Returns(user);

            var result = await _userService.DeactivateMe(userId);

            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task DeactivateMe_WhenUserDoesNotExists_ReturnFail()
        {
            int userId = 99;
            _userRepositoryMock.GetUserByIdAsync(userId).Returns((User)null);

            var result = await _userService.DeactivateMe(userId);

            Assert.True(result.IsFailed);
            Assert.Equal($"User not found by id {userId}.", result.Errors.First().Message);

            await _userRepositoryMock.DidNotReceive().DeactivateUser(Arg.Any<User>());
        }

        // UpdateUser
        [Fact]
        public async Task UpdateUser_WhenUserIsNotOwner_ReturnFail()
        {
            int userIdToUpdate = 100;
            int loggedUserId = 1;

            var dto = new UpdateUserRequestDTO { Name = "New Name" };
            var user = new User { Id = userIdToUpdate, Name = "Old Name", Email = "user@email.com", Password = "password123" };

            _userRepositoryMock.GetUserByIdAsync(userIdToUpdate).Returns(user);

            var result = await _userService.UpdateUser(userIdToUpdate, dto, loggedUserId);

            Assert.True(result.IsFailed);
            Assert.Equal("You cannot update other users data.", result.Errors.First().Message);

            await _userRepositoryMock.DidNotReceive().UpdateUser(Arg.Any<User>());
        }

        [Fact]
        public async Task UpdateUser_WhenUserDoesNotExists_ReturnFail()
        {
            int userId = 99;
            int loggedUserId = 1;

            var dto = new UpdateUserRequestDTO { Name = "New Name" };

            _userRepositoryMock.GetUserByIdAsync(userId).Returns((User)null);

            var result = await _userService.UpdateUser(userId, dto, 1);

            Assert.True(result.IsFailed);
            Assert.Equal($"User not found by id {userId}.", result.Errors.First().Message);

            await _userRepositoryMock.DidNotReceive().UpdateUser(Arg.Any<User>());
        }

        [Fact]
        public async Task UpdateUser_WhenUserExistsAndIsOwner_ReturnSuccess()
        {
            int userId = 1;
            var dto = new UpdateUserRequestDTO { Name = "Updated Name" };
            var user = new User { Id = userId, Name = "Old Name", Email = "user@email.com", Password = "password123" };

            _userRepositoryMock.GetUserByIdAsync(userId).Returns(user);

            var result = await _userService.UpdateUser(userId, dto, userId);

            Assert.True(result.IsSuccess);
            Assert.Equal("Updated Name", result.Value.Name);

            await _userRepositoryMock.Received(1).UpdateUser(Arg.Is<User>(u => u.Name == "Updated Name"));
        }
    }
}
