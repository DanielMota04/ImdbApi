using Application.DTOs.Response.Movie;
using Application.DTOs.Response.User;
using Application.Interfaces;
using Domain.Enums;
using Domain.Interface.Repositories;
using Domain.Models;

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
        // AddMovieToList when movie does not exists return fail
        // AddMovieToList when user does not exists return fail
        // AddMovieToList when movie and user exists return success

        // RemoveMovieFromList
        // RemoveMovieFromList when movie is not in list return fail
        // RemoveMovieFromList when movie is in another users list return fail
        // RemoveMovieFromList when movie is in users lists return success


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
