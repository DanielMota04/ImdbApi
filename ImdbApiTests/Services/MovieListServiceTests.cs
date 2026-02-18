using Application.DTOs.Response.Movie;
using Application.DTOs.Response.User;
using Application.Interfaces;
using Domain.Enums;
using Domain.Errors;
using Domain.Interface.Repositories;
using Domain.Models;
using FluentResults;

namespace ImdbApiTests.Services
{
    public class MovieListServiceTests
    {
        private readonly IMovieService _movieServiceMock;
        private readonly IUserService _userServiceMock;
        private readonly IMovieListRepository _movieListRepositoryMock;
        private readonly IMovieRepository _movieRepositoryMock;

        private readonly MovieListService _movieListService;

        public MovieListServiceTests()
        {
            _movieServiceMock = Substitute.For<IMovieService>();
            _userServiceMock = Substitute.For<IUserService>();
            _movieListRepositoryMock = Substitute.For<IMovieListRepository>();
            _movieRepositoryMock = Substitute.For<IMovieRepository>();
            _movieListService = new MovieListService(_movieServiceMock, _userServiceMock, _movieListRepositoryMock, _movieRepositoryMock);
        }

        // AddMovieToList
        [Fact]
        public async Task AddMovieToList_WhenMovieDoesNotExists_ReturnFail()
        {
            int movieId = 99;
            int userId = 10;
            _movieServiceMock.GetMovieById(movieId).Returns(Result.Fail<MovieDetailsResponseDTO>("Movie not found"));

            var result = await _movieListService.AddMovieToList(movieId, userId);

            Assert.True(result.IsFailed);
            Assert.Equal("Movie not found", result.Errors.First().Message);

            await _movieListRepositoryMock
                .DidNotReceive()
                .CreateMovieList(Arg.Any<MovieList>());
        }

        [Fact]
        public async Task AddMovieToList_WhenUserDoesNotExists_ReturnFail()
        {
            int movieId = 1;
            int userId = 99;
            _movieServiceMock.GetMovieById(movieId).Returns(movie);
            _userServiceMock.GetUserById(userId).Returns(Result.Fail<UserResponse>("User not found"));

            var result = await _movieListService.AddMovieToList(movieId, userId);

            Assert.True(result.IsFailed);
            Assert.Equal("User not found", result.Errors.First().Message);

            await _movieListRepositoryMock
                .DidNotReceive()
                .CreateMovieList(Arg.Any<MovieList>());
        }
        [Fact]
        public async Task AddMovieToList_WhenMovieExists_ReturnsDTO()
        {
            int movieId = 1;
            int userId = 10;
            _movieServiceMock.GetMovieById(movieId).Returns(movie);
            _userServiceMock.GetUserById(userId).Returns(user);

            var result = await _movieListService.AddMovieToList(movieId, userId);

            Assert.True(result.IsSuccess);
            Assert.NotNull(result);

            await _movieListRepositoryMock
                .Received(1)
                .CreateMovieList(Arg.Any<MovieList>());
        }

        // RemoveMovieFromList
         [Fact]
        public async Task RemoveMovieFromList_WhenMovieIsNotOnList_ReturnFail()
        {
            int movieListId = 99;
            int userId = 99;

            _movieListRepositoryMock.FindMovieListById(movieListId).Returns(null as MovieList);

            var result = await _movieListService.RemoveMovieFromList(movieListId, userId);

            Assert.True(result.IsFailed);
            Assert.Equal("Movie List not found", result.Errors.First().Message);

            _movieListRepositoryMock
                .DidNotReceive()
                .RemoveMovieFromList(Arg.Any<MovieList>());
        }
        [Fact]
        public async Task RemoveMovieFromList_WhenMovieIsOnAnotherUserList_ReturnFail()
        {
            int movieListId = movieList.MovieListId;
            int userId = 99;

            _movieListRepositoryMock.FindMovieListById(movieListId).Returns(movieList);

            var result = await _movieListService.RemoveMovieFromList(movieListId, userId);

            Assert.True(result.IsFailed);
            Assert.Equal("You can't remove a movie that is not in your list", result.Errors.First().Message);

            _movieListRepositoryMock
                .DidNotReceive()
                .RemoveMovieFromList(Arg.Any<MovieList>());
        }
        [Fact]
        public async Task RemoveMovieFromList_WhenMovieIsOnUserList_ReturnSuccess()
        {
            int movieListId = movieList.MovieListId;
            int userId = movieList.UserId;

            _movieListRepositoryMock.FindMovieListById(movieListId).Returns(movieList);

            var result = await _movieListService.RemoveMovieFromList(movieListId, userId);

            Assert.True(result.IsSuccess);

            _movieListRepositoryMock
                .Received(1)
                .RemoveMovieFromList(Arg.Any<MovieList>());
        }


        private MovieList movieList = new()
        {
            MovieId = 5,
            UserId = 10
        };

        private UserResponse user = new()
        {
            Id = 1,
            Name = "username",
            Role = Roles.Admin
        };

        private MovieDetailsResponseDTO movie = new()
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
    }
}
