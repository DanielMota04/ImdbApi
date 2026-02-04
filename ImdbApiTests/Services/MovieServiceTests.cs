using ImdbApi.DTOs.Request.Movie;
using ImdbApi.DTOs.Response.Movie;
using ImdbApi.Interfaces.Repositories;
using ImdbApi.Mappers;
using ImdbApi.Models;
using ImdbApi.Services;
using Microsoft.AspNetCore.Http;


namespace ImdbApiTests.Services
{
    public class MovieServiceTests
    {
        private readonly MovieMapper _mapper;
        private readonly Mock<IMovieRepository> _movieRepositoryMock;
        private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private readonly Mock<IMovieListRepository> _movieListRepositoryMock;

        private readonly MovieService _movieService;

        public MovieServiceTests()
        {
            _mapper = new MovieMapper();
            _movieRepositoryMock = new Mock<IMovieRepository>();
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            _movieListRepositoryMock = new Mock<IMovieListRepository>();

            _movieService = new MovieService
                (_mapper, _movieRepositoryMock.Object, _httpContextAccessorMock.Object, _movieListRepositoryMock.Object);
        }

        [Fact]
        public async Task CreateMovie_WhenTitleExists_ReturnNull()
        {
            var movie = new CreateMovieRequestDTO
            {
                Title = "O poderoso chefão",
                Genre = "Drama",
                Director = "Francis ford copolla",
                Actors = new List<string> {
"Marlon Brando", "Al Pacino", "James Caan" }
            };
            _movieRepositoryMock.Setup(repo => repo.FindMovieByTitle("o poderoso chefão")).ReturnsAsync(true);

            var result = await _movieService.CreateMovie(movie);

            Assert.Null(result);
        }

        [Fact]
        public async Task CreateMovie_WhenTitleDontExist_ReturnMovie()
        {
            var dto = new CreateMovieRequestDTO
            {
                Title = "O poderoso chefão",
                Genre = "Drama",
                Director = "Francis ford copolla",
                Actors = new List<string> {
"Marlon Brando", "Al Pacino", "James Caan" }
            };

            var movie = new MovieDetailsResponseDTO
        {
            Id = 0,
            Title = "O poderoso chefão",
            Rating = 0.0,
            Genre = "Drama",
            Director = "Francis ford copolla",
            Actors = new List<string> {
"Marlon Brando", "Al Pacino", "James Caan" }
        };

            _movieRepositoryMock.Setup(repo => repo.FindMovieByTitle("o poderoso chefão")).ReturnsAsync(false);

            var result = await _movieService.CreateMovie(dto);

            Assert.Equal(movie, result);
        }
    }
}
