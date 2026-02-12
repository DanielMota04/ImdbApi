using Application.DTOs.Response.Movie;
using Application.DTOs.Response.User;
using Application.Interfaces;
using Application.Services;
using Domain.Enums;
using Domain.Exceptions;
using Domain.Interface.Repositories;
using Domain.Models;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace ImdbApiTests.Services
{
    public class MovieListServiceTests
    {
        private readonly Mock<IMovieService> _movieServiceMock;
        private readonly Mock<IUserService> _userServiceMock;
        private readonly Mock<IMovieListRepository> _movieListRepositoryMock;
        private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;

        private readonly MovieListService _movieListService;

        public MovieListServiceTests()
        {
            _movieServiceMock = new Mock<IMovieService>();
            _userServiceMock = new Mock<IUserService>();
            _movieListRepositoryMock = new Mock<IMovieListRepository>();
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            _movieListService = new MovieListService(_movieServiceMock.Object, _userServiceMock.Object, _movieListRepositoryMock.Object, _httpContextAccessorMock.Object);
        }

        private void MockUserLogin(string userId)
        {
            var context = new DefaultHttpContext();
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, userId)
            };

            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            context.User = claimsPrincipal;

            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(context);
        }

        [Fact]
        public async Task AddMovieToList_WhenMovieDoesNotExists_ThrowException()
        {
            int userId = 5;
            int movieId = 99;
            MockUserLogin(userId.ToString());

            MovieList movieList = new()
            {
                MovieId = movieId,
                UserId = userId
            };

            UserResponse user = new()
            {
                Id = userId,
                Name = "username",
                Role = Roles.Admin
            };

            _movieServiceMock.Setup(service => service.GetMovieById(movieId)).ReturnsAsync((MovieDetailsResponseDTO)null);
            _userServiceMock.Setup(service => service.GetUserById(userId)).ReturnsAsync(user);

            await Assert.ThrowsAsync<ResourceNotFoundException>(() => _movieListService.AddMovieToList(movieId));

            _movieListRepositoryMock.Verify(x => x.CreateMovieList(It.IsAny<MovieList>()), Times.Never);
        }

        [Fact]
        public async Task AddMovieToList_WhenMovieExists_ReturnsDTO()
        {
            int userId = 5;
            int movieId = 10;
            MockUserLogin(userId.ToString());

            MovieList movieList = new()
            {
                MovieListId = 1,
                MovieId = movieId,
                UserId = userId
            };

            UserResponse user = new()
            {
                Id = userId,
                Name = "username",
                Role = Roles.Admin
            };

            MovieDetailsResponseDTO movie = new()
            {
                Title = "O poderoso chefão",
                Genre = "Drama",
                Rating = 0.0,
                Director = "Francis ford copolla",
                Actors = new List<string>
                {
                    "Marlon Brando", "Al Pacino", "James Caan"
                }
            };

            _movieServiceMock.Setup(service => service.GetMovieById(movieId)).ReturnsAsync(movie);
            _userServiceMock.Setup(service => service.GetUserById(userId)).ReturnsAsync(user);
            _movieListRepositoryMock.Setup(repo => repo.CreateMovieList(It.IsAny<MovieList>())).ReturnsAsync(movieList);

            var result = await _movieListService.AddMovieToList(movieId);

            Assert.NotNull(result);
            Assert.Equal(0, result.MovieListId);
            Assert.Equal(movieId, result.MovieId);
            Assert.Equal(user.Name, result.Username);

            _movieListRepositoryMock.Verify(x => x.CreateMovieList(It.IsAny<MovieList>()), Times.Once);
        }

        [Fact]
        public async Task RemoveMovieFromList_WhenMovieDoesNotExists_ThrowException()
        {
            int userId = 5;
            int movieListId = 1;
            MockUserLogin(userId.ToString());

            _movieListRepositoryMock.Setup(repo => repo.FindMovieListById(movieListId)).ReturnsAsync((MovieList) null);

            await Assert.ThrowsAsync<ResourceNotFoundException>(() => _movieListService.RemoveMovieFromList(movieListId));

            _movieListRepositoryMock.Verify(x => x.RemoveMovieFromList(It.IsAny<MovieList>()), Times.Never);
        }

        [Fact]
        public async Task RemoveMovieFromList_WhenMovieIsNotOnUserList_ThrowException()
        {
            int loggedUserId = 5;
            int listUserId = 10;
            int movieId = 10;
            int movieListId = 1;
            MockUserLogin(loggedUserId.ToString());

            MovieList movieList = new()
            {
                MovieListId = 1,
                MovieId = movieId,
                UserId = listUserId
            };

            _movieListRepositoryMock.Setup(repo => repo.FindMovieListById(movieListId)).ReturnsAsync(movieList);

            await Assert.ThrowsAsync<ForbiddenException>(() => _movieListService.RemoveMovieFromList(movieListId));

            _movieListRepositoryMock.Verify(x => x.RemoveMovieFromList(It.IsAny<MovieList>()), Times.Never);
        }

        [Fact]
        public async Task RemoveMovieFromList_WhenMovieExistsAndIsOnUserList_ReturnTrue()
        {
            int userId = 5;
            int movieId = 10;
            int movieListId = 1;
            MockUserLogin(userId.ToString());

            MovieList movieList = new()
            {
                MovieListId = 1,
                MovieId = movieId,
                UserId = userId
            };

            _movieListRepositoryMock.Setup(repo => repo.FindMovieListById(movieListId)).ReturnsAsync(movieList);

            var result = await _movieListService.RemoveMovieFromList(movieListId);

            Assert.True(result);

            _movieListRepositoryMock.Verify(x => x.RemoveMovieFromList(It.IsAny<MovieList>()), Times.Once);
        }
    }
}
