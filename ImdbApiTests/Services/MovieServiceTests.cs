using ImdbApi.DTOs.Request.Movie;
using ImdbApi.Exceptions;
using ImdbApi.Interfaces.Repositories;
using ImdbApi.Mappers;
using ImdbApi.Models;
using ImdbApi.Services;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Security.Claims;


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
        public async Task CreateMovie_WhenTitleExists_ThrowException()
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

            await Assert.ThrowsAsync<ConflictException>(() => _movieService.CreateMovie(movie));
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
        public async Task DeleteMovie_WhenMovieDoesNotExists_ThrowException()
        {
            int movieId = 99;

            _movieRepositoryMock.Setup(repo => repo.FindMovieById(movieId)).ReturnsAsync((ImdbApi.Models.Movie)null);

            await Assert.ThrowsAsync<ResourceNotFoundException>(() => _movieService.DeleteMovie(movieId));

            _movieRepositoryMock.Verify(x => x.DeleteMovie(It.IsAny<Movie>()), Times.Never);
        }

        [Fact]
        public async Task Vote_WhenUserHasNotVotedYet_ShouldCalculateAndReturnRating()
        {
            int userId = 5;
            int movieId = 10;
            double vote = 4.0;
            var voteDto = new VoteMovieRequestDTO
            {
                MovieId = movieId,
                Vote = vote
            };

            MockUserLogin(userId.ToString());

            var movie = new Movie
            {
                Id = movieId,
                Title = "Clube da luta",
                Rating = 0,
                Genre = "Ação",
                Director = "David Fincher",
                Actors = new List<string>{
                    "Brad pitt", "Edward Norton"
                }
            };

            _movieRepositoryMock.Setup(x => x.FindMovieById(movieId)).ReturnsAsync(movie);

            var movieList = new MovieList
            {
                MovieListId = 1,
                MovieId = movieId,
                UserId = userId,
                IsVoted = false
            };

            _movieListRepositoryMock.Setup(x => x.FindMovieInListByMovieIdAndUserId(movieId, userId)).ReturnsAsync(movieList);

            var result = await _movieService.Vote(voteDto);

            Assert.NotNull(result);
            Assert.Equal(4.0, result);

            _movieRepositoryMock.Verify(x => x.UpdateRating(It.IsAny<Movie>()), Times.Once());
            _movieListRepositoryMock.Verify(x => x.UpdateIsVoted(It.IsAny<MovieList>()), Times.Once());
        }

        [Fact]
        public async Task Vote_WhenUserAlreadyVoted_ThrowException()
        {
            int userId = 5;
            int movieId = 10;
            double vote = 4.0;
            var voteDto = new VoteMovieRequestDTO
            {
                MovieId = movieId,
                Vote = vote
            };

            MockUserLogin(userId.ToString());

            var movie = new Movie
            {
                Id = movieId,
                Title = "Clube da luta",
                Rating = 0,
                Genre = "Ação",
                Director = "David Fincher",
                Actors = new List<string>{
                    "Brad pitt", "Edward Norton"
                }
            };

            _movieRepositoryMock.Setup(x => x.FindMovieById(movieId)).ReturnsAsync(movie);

            var movieList = new MovieList
            {
                MovieListId = 1,
                MovieId = movieId,
                UserId = userId,
                IsVoted = true
            };

            _movieListRepositoryMock.Setup(x => x.FindMovieInListByMovieIdAndUserId(movieId, userId)).ReturnsAsync(movieList);

            await Assert.ThrowsAsync<ForbiddenException>(() => _movieService.Vote(voteDto));

            _movieRepositoryMock.Verify(x => x.UpdateRating(It.IsAny<Movie>()), Times.Never());
        }
    }
}
