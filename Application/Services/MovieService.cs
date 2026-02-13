using Application.DTOs.Request.Movie;
using Application.DTOs.Response.Movie;
using Application.Interfaces;
using Application.Mappers;
using Application.Validators;
using Domain.Enums;
using Domain.Interface.Repositories;
using FluentValidation;
using Domain.Models.Pagination;
using FluentResults;
using Domain.Errors;

namespace Application.Services
{
    public class MovieService : IMovieService
    {
        private readonly IMovieRepository _movieRepository;
        private readonly IMovieListRepository _movieListRepository;

        public MovieService(IMovieRepository movieRepository, IMovieListRepository movieListRepository)
        {
            _movieRepository = movieRepository;
            _movieListRepository = movieListRepository;
        }

        public async Task<Result<MovieDetailsResponseDTO>> CreateMovie(CreateMovieRequestDTO dto)
        {
            CreateMovieValidator validator = new();
            var title = dto.Title.Trim().Normalize();
            var genre = dto.Genre.Trim().Normalize();
            var director = dto.Director.Trim().Normalize();

            var actors = dto.Actors.Select(a => a.Trim().Normalize()).ToList();

            var movieExistsByTitle = await _movieRepository.FindMovieByTitle(title.ToLower());
            if (movieExistsByTitle)
                return Result.Fail(new ConflictError("Movie name already exists."));

            validator.ValidateAndThrow(dto);

            var movie = MovieMapper.CreateToEntity(title, genre, actors, director);
            await _movieRepository.CreateMovie(movie);

            return Result.Ok(MovieMapper.EntityToDetails(movie));
        }

        public async Task<Result<PagedResult<MovieResponseDTO>>> GetAllMovies(PaginationParams paginationParams, string? title, string? director, string? genre, string? actor, MovieOrderBy order)
        {
            var movies = await _movieRepository.GetAllMovies(paginationParams, title, director, genre, actor, order);

            var mmappedMovies = movies.Items?.Select(m => MovieMapper.EntityToResponse(m)).ToList() ?? new List<MovieResponseDTO>();

            return Result.Ok(new PagedResult<MovieResponseDTO>
            {
                Items = mmappedMovies,
                TotalItems = movies.TotalItems,
                PageNumber = movies.PageNumber,
                PageSize = movies.PageSize
            });
        }

        public async Task<Result<MovieDetailsResponseDTO>> GetMovieById(int id)
        {
            var movie = await _movieRepository.FindMovieById(id);
            if (movie == null)
                return Result.Fail(new NotFoundError($"Movie not found with id {id}."));
            
            return Result.Ok(MovieMapper.EntityToDetails(movie));
        }

        public async Task<Result<bool>> DeleteMovie(int id)
        {
            var movie = await _movieRepository.FindMovieById(id);

            if (movie == null)
                return Result.Fail(new NotFoundError($"Movie not found with id {id}."));

            _movieRepository.DeleteMovie(movie);

            return Result.Ok(true);
        }

        public async Task<Result<double>> Vote(VoteMovieRequestDTO vote, int userId)
        {
            VoteValidator validator = new VoteValidator();

            var movie = await _movieRepository.FindMovieById(vote.MovieId);

            if (movie is null)
                return Result.Fail(new NotFoundError("Movie not found in your list."));

            var movieList = await _movieListRepository.FindMovieInListByMovieIdAndUserId(vote.MovieId, userId);

            if (movieList is null)
                return Result.Fail(new NotFoundError("Movie not found in your list."));

            if (movieList.IsVoted)
                return Result.Fail(new ForbiddenError("You has already voted in this movie."));

            validator.ValidateAndThrow(vote);

            movie.TotalRating += vote.Vote;
            movie.Votes += 1;
            movie.Rating = movie.TotalRating / movie.Votes;
            _movieListRepository.UpdateIsVoted(movieList);

            _movieRepository.UpdateRating(movie);

            return Result.Ok(movie.Rating);
        }
    }
}
