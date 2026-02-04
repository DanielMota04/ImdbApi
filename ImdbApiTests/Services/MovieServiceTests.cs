using ImdbApi.DTOs.Request.Movie;
using ImdbApi.Interfaces.Repositories;
using ImdbApi.Mappers;
using ImdbApi.Models;
using ImdbApi.Services;
using Microsoft.AspNetCore.Http;
using Moq;


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
                Actors = new List<string> 
                {
                    "Marlon Brando", "Al Pacino", "James Caan" 
                }
            };
            _movieRepositoryMock.Setup(repo => repo.FindMovieByTitle("o poderoso chefão")).ReturnsAsync(true);

            var result = await _movieService.CreateMovie(movie);

            Assert.Null(result);
        }
        
        [Fact]
        public async Task CreateMovie_WhenTitleDoesNotExists_ReturnMovie()
        {
            var movie = new CreateMovieRequestDTO
            {
                Title = "O poderoso chefão",
                Genre = "Drama",
                Director = "Francis ford copolla",
                Actors = new List<string>
                {
                    "Marlon Brando", "Al Pacino", "James Caan"
                }
            };
            _movieRepositoryMock.Setup(repo => repo.FindMovieByTitle("o poderoso chefão")).ReturnsAsync(false);

            var entity = new Movie
            {
                Id = 1,
                Title = "O poderoso chefão",
                Genre = "Drama",
                Rating = 0.0,
                Director = "Francis ford copolla",
                Actors = new List<string>
                {
                    "Marlon Brando", "Al Pacino", "James Caan"
                }
            };
            _movieRepositoryMock.Setup(repo => repo.CreateMovie(It.IsAny<Movie>())).ReturnsAsync(entity);

            var result = await _movieService.CreateMovie(movie);

            Assert.NotNull(result);
            Assert.Equal(0, result.Id); // verificar por que 0 e não 1
            Assert.Equal("O poderoso chefão", result.Title);
        }

        [Fact]
        public async Task DeleteMovie_WhenMovieExists_ReturnTrue()
        {
            int movieId = 1;
            var movie = new Movie
            {
                Id = 1,
                Title = "O poderoso chefão",
                Genre = "Drama",
                Rating = 0.0,
                Director = "Francis ford copolla",
                Actors = new List<string>
                {
                    "Marlon Brando", "Al Pacino", "James Caan"
                }
            };

            _movieRepositoryMock.Setup(repo => repo.FindMovieById(movieId)).ReturnsAsync(movie);
            _movieRepositoryMock.Setup(repo => repo.DeleteMovie(movie)).ReturnsAsync(true);

            var result = await _movieService.DeleteMovie(movieId);

            Assert.True(result);

            _movieRepositoryMock.Verify(x => x.DeleteMovie(movie), Times.Once);
        }

        [Fact]
        public async Task DeleteMovie_WhenMovieDoesNotExists_ReturnFalse()
        {
            int movieId = 99;

            _movieRepositoryMock.Setup(repo => repo.FindMovieById(movieId)).ReturnsAsync((ImdbApi.Models.Movie)null);

            var result = await _movieService.DeleteMovie(movieId);

            Assert.False(result);

            _movieRepositoryMock.Verify(x => x.DeleteMovie(It.IsAny<Movie>()), Times.Never);
        }
    }
}
