using Application.DTOs.Pagination;
using Application.DTOs.Request.Movie;
using Application.DTOs.Response.Movie;
using Application.Interfaces;
using Application.Mappers;
using Application.Validators;
using Domain.Enums;
using Domain.Interface.Repositories;
using Domain.Models;
using FluentValidation;
using Domain.Exceptions;

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

        public async Task<MovieDetailsResponseDTO> CreateMovie(CreateMovieRequestDTO dto)
        {
            CreateMovieValidator validator = new();
            var title = dto.Title.Trim().Normalize();
            var genre = dto.Genre.Trim().Normalize();
            var director = dto.Director.Trim().Normalize();

            var actors = dto.Actors.Select(a => a.Trim().Normalize()).ToList();

            var movieExistsByTitle = await _movieRepository.FindMovieByTitle(title.ToLower());
            if (movieExistsByTitle)
                throw new ConflictException("Movie name already exists.");

            validator.ValidateAndThrow(dto);

            var movie = MovieMapper.CreateToEntity(title, genre, actors, director);
            await _movieRepository.CreateMovie(movie);

            return MovieMapper.EntityToDetails(movie);
        }

        public async Task<PagedResult<MovieResponseDTO>> GetAllMovies(PaginationParams paginationParams, string? title, string? director, string? genre, string? actor, MovieOrderBy order)
        {
            var allMovies = await _movieRepository.GetAllMovies();
            var query = allMovies.AsQueryable();

            if (title != null)
            {
                query = query.Where(m => m.Title.Contains(title));
            }
            if (director != null)
            {
                query = query.Where(m => m.Director.Contains(director));
            }
            if (genre != null)
            {
                query = query.Where(m => m.Genre.Contains(genre));
            }
            if (actor != null)
            {
                query = query.Where(m => m.Actors.Contains(actor));
            }

            var totalItems = query.Count();

            List<Movie> pagedMovies = new List<Movie>();

            if (order.ToString().Equals("Rating"))
            {
                pagedMovies = query.OrderByDescending(m => m.Rating)
                    .Skip((paginationParams.PageNumber - 1) * paginationParams.PageSize)
                    .Take(paginationParams.PageSize)
                    .ToList();
            }
            else if (order.ToString().Equals("Alphabetic"))
            {
                pagedMovies = query.OrderBy(m => m.Title)
                    .Skip((paginationParams.PageNumber - 1) * paginationParams.PageSize)
                    .Take(paginationParams.PageSize)
                    .ToList();
            }

            var mmappedMovies = pagedMovies.Select(m => MovieMapper.EntityToResponse(m));

            return new PagedResult<MovieResponseDTO>
            {
                Items = mmappedMovies,
                TotalItems = totalItems,
                PageNumber = paginationParams.PageNumber,
                PageSize = paginationParams.PageSize
            };
        }

        public async Task<MovieDetailsResponseDTO> GetMovieById(int id)
        {
            var movie = await _movieRepository.FindMovieById(id);
            if (movie == null) 
                throw new ResourceNotFoundException($"Movie not found with id {id}.");
            
            return MovieMapper.EntityToDetails(movie);
        }

        public async Task<bool> DeleteMovie(int id)
        {
            var movie = await _movieRepository.FindMovieById(id);

            if (movie == null) 
                throw new ResourceNotFoundException($"Movie not found with id {id}.");
            
            await _movieRepository.DeleteMovie(movie);

            return true;
        }

        public async Task<double?> Vote(VoteMovieRequestDTO vote, int userId)
        {
            VoteValidator validator = new VoteValidator();

            var movie = await _movieRepository.FindMovieById(vote.MovieId);
            var movieList = await _movieListRepository.FindMovieInListByMovieIdAndUserId(vote.MovieId, userId);

            if (movieList.IsVoted) 
                throw new ForbiddenException("You has already voted in this movie.");

            validator.ValidateAndThrow(vote);

            movie.TotalRating += vote.Vote;
            movie.Votes += 1;
            movie.Rating = movie.TotalRating / movie.Votes;
            await _movieListRepository.UpdateIsVoted(movieList);

            await _movieRepository.UpdateRating(movie);

            return movie.Rating;
        }
    }
}
