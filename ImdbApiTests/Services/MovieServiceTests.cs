using Domain.Interface.Repositories;
using Domain.Models;
using Application.DTOs.Request.Movie;


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
        // CreateMovie when name already exists return fail
        // CreateMovie when name does not exists return success

        // GetMovieById
        // GetMovieById when movie does Not exists return fail
        // GetMovieById when movie exists return success

        // DeleteMovie
        // DeleteMovie when movie does Not exists return fail
        // DeleteMovie when movie exists return success

        // Vote
        // Vote when movie is null return fail
        // Vote when movieList is null return fail
        // Vote when movie is already voted return fail
        // Vote -- return success

        private Movie movie = new()
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

        private CreateMovieRequestDTO createMovieRequest = new()
        {
            Title = "O poderoso chefão",
            Genre = "Drama",
            Director = "Francis ford copolla",
            Actors = new List<string>
                {
                    "Marlon Brando", "Al Pacino", "James Caan"
                }
        };

        private VoteMovieRequestDTO voteMovieRequestDTO = new()
        {
            MovieId = 10,
            Vote = 4.0
        };

    }
}
