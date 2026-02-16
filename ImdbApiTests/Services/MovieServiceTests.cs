using Application.DTOs.Request.Movie;
using Domain.Interface.Repositories;
using Domain.Models;

namespace ImdbApiTests.Services
{
    public class MovieServiceTests
    {
        private readonly IMovieRepository _movieRepositoryMock;
        private readonly IMovieListRepository _movieListRepositoryMock;

        private readonly MovieService _movieService;

        public MovieServiceTests()
        {
            _movieRepositoryMock = Substitute.For<IMovieRepository>();
            _movieListRepositoryMock = Substitute.For<IMovieListRepository>();

            _movieService = new MovieService
                (_movieRepositoryMock, _movieListRepositoryMock);
        }

        // CreateMovie
        [Fact]
        public async Task CreateMovie_WhenTitleAlreadyExists_ReturnFail()
        {
            _movieRepositoryMock.FindMovieByTitle("o poderoso chefão").Returns(true);

            var result = await _movieService.CreateMovie(createMovieRequestDTO);

            Assert.True(result.IsFailed);
            Assert.Equal("Movie name already exists.", result.Errors.First().Message);

            await _movieRepositoryMock
                .DidNotReceive()
                .CreateMovie(Arg.Any<Movie>());
        }

        [Fact]
        public async Task CreateMovie_WhenTitleDoesNotExists_ReturnMovie()
        {
            _movieRepositoryMock.FindMovieByTitle("o poderoso chefão").Returns(false);

            _movieRepositoryMock.CreateMovie(Arg.Any<Movie>()).Returns(movieEntity);

            var result = await _movieService.CreateMovie(createMovieRequestDTO);

            Assert.True(result.IsSuccess);
            Assert.NotNull(result);
            Assert.Equal(0, result.Value.Id);
            Assert.Equal("O poderoso chefão", result.Value.Title);

            await _movieRepositoryMock
                .Received(1)
                .CreateMovie(Arg.Any<Movie>());
        }

        // GetMovieById
        [Fact]
        public async Task GetMovieById_WhenMovieDoesNotExists_ReturnFail()
        {
            int movieId = 99;
            _movieRepositoryMock.FindMovieById(movieId).Returns(null as Movie);

            var result = await _movieService.GetMovieById(movieId);

            Assert.True(result.IsFailed);

            await _movieRepositoryMock
                .Received(1)
                .FindMovieById(movieId);
        }

        [Fact]
        public async Task GetMovieById_WhenMovieExists_ReturnMovie()
        {
            int movieId = movieEntity.Id;
            _movieRepositoryMock.FindMovieById(movieId).Returns(movieEntity);

            var result = await _movieService.GetMovieById(movieId);

            Assert.True(result.IsSuccess);
            Assert.NotNull(result);
            Assert.Equal(movieId, result.Value.Id);
            Assert.Equal(movieEntity.Title, result.Value.Title);

            await _movieRepositoryMock
                .Received(1)
                .FindMovieById(movieId);
        }

        // DeleteMovie
        [Fact]
        public async Task DeleteMovie_WhenMovieDoesNotExists_ReturnFail()
        {
            int movieId = 99;
            _movieRepositoryMock.FindMovieById(movieId).Returns(null as Movie);

            var result = await _movieService.DeleteMovie(movieId);

            Assert.True(result.IsFailed);
            Assert.Equal($"Movie not found with id {movieId}.", result.Errors.First().Message);

            _movieRepositoryMock
                .DidNotReceive()
                .DeleteMovie(Arg.Any<Movie>());
        }

        [Fact]
        public async Task DeleteMovie_WhenMovieExists_ReturnSuccess()
        {
            int movieId = movieEntity.Id;
            _movieRepositoryMock.FindMovieById(movieId).Returns(movieEntity);

            var result = await _movieService.DeleteMovie(movieId);

            Assert.True(result.IsSuccess);

            _movieRepositoryMock
                .Received(1)
                .DeleteMovie(Arg.Any<Movie>());
        }


        // Vote
        [Fact]
        public async Task Vote_WhenMovieDoesNotExists_ReturnFail()
        {
            int movieId = voteMovieRequestDTO.MovieId;
            int userId = 1;
            _movieRepositoryMock.FindMovieById(movieId).Returns(null as Movie);

            var result = await _movieService.Vote(voteMovieRequestDTO, userId);

            Assert.True(result.IsFailed);
            Assert.Equal("Movie not found in your list.", result.Errors.First().Message);

            _movieRepositoryMock
                .DidNotReceive()
                .UpdateRating(Arg.Any<Movie>());
        }

        [Fact]
        public async Task Vote_WhenMovieIsNotOnList_ReturnFail()
        {
            int movieId = voteMovieRequestDTO.MovieId;
            int userId = 1;
            _movieRepositoryMock.FindMovieById(movieId).Returns(movieEntity);

            _movieListRepositoryMock.FindMovieInListByMovieIdAndUserId(movieId, userId).Returns(null as MovieList);

            var result = await _movieService.Vote(voteMovieRequestDTO, userId);

            Assert.True(result.IsFailed);
            Assert.Equal("Movie not found in your list.", result.Errors.First().Message);

            _movieRepositoryMock
                .DidNotReceive()
                .UpdateRating(Arg.Any<Movie>());
        }

        [Fact]
        public async Task Vote_WhenUserAlreadyVoted_ReturnFail()
        {
            int movieId = voteMovieRequestDTO.MovieId;
            int userId = 1;
            _movieRepositoryMock.FindMovieById(movieId).Returns(movieEntity);

            _movieListRepositoryMock.FindMovieInListByMovieIdAndUserId(movieId, userId).Returns(movieVoted);

            var result = await _movieService.Vote(voteMovieRequestDTO, userId);

            Assert.True(result.IsFailed);
            Assert.Equal("You has already voted in this movie.", result.Errors.First().Message);

            _movieRepositoryMock
                .DidNotReceive()
                .UpdateRating(Arg.Any<Movie>());
        }

        [Fact]
        public async Task Vote_WhenUserHasNotVotedYet_ShouldCalculateAndReturnRating()
        {
            int movieId = voteMovieRequestDTO.MovieId;
            int userId = 1;
            _movieRepositoryMock.FindMovieById(movieId).Returns(movieEntity);

            _movieListRepositoryMock.FindMovieInListByMovieIdAndUserId(movieId, userId).Returns(movieNotVoted);

            var result = await _movieService.Vote(voteMovieRequestDTO, userId);

            Assert.True(result.IsSuccess);
            Assert.Equal(4.0, result.Value);

            _movieRepositoryMock
                .Received(1)
                .UpdateRating(Arg.Any<Movie>());

            _movieListRepositoryMock
                .Received(1)
                .UpdateIsVoted(Arg.Any<MovieList>());

        }

        private readonly Movie movieEntity = new()
        {
            Id = 1,
            Title = "O poderoso chefão",
            Genre = "Drama",
            Rating = 0.0,
            Director = "Francis ford copolla",
            Actors =
                [
                    "Marlon Brando", "Al Pacino", "James Caan"
                ]
        };

        private readonly CreateMovieRequestDTO createMovieRequestDTO = new()
        {
            Title = "O poderoso chefão",
            Genre = "Drama",
            Director = "Francis ford copolla",
            Actors = new List<string>
                {
                    "Marlon Brando", "Al Pacino", "James Caan"
                }
        };

        private readonly VoteMovieRequestDTO voteMovieRequestDTO = new()
        {
            MovieId = 10,
            Vote = 4.0
        };

        private readonly MovieList movieVoted = new()
        {
            MovieListId = 1,
            UserId = 1,
            MovieId = 10,
            IsVoted = true
        };

        private readonly MovieList movieNotVoted = new()
        {
            MovieListId = 1,
            UserId = 1,
            MovieId = 10,
            IsVoted = false
        };

    }
}
