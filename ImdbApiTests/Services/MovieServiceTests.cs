using ImdbApi.DTOs.Request.Movie;
using ImdbApi.Interfaces.Repositories;
using ImdbApi.Mappers;
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

    }
}
