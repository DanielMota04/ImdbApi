using Application.DTOs.Pagination;
using Application.DTOs.Response.Movie;
using Application.Interfaces;
using Application.Mappers;
using Domain.Interface.Repositories;
using Domain.Models;
using Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Application.Services
{
    public class MovieListService : IMovieListService
    {
        private readonly IMovieService _movieService;
        private readonly IUserService _userService;
        private readonly IMovieListRepository _movieListRepository;

        public MovieListService(IMovieService movieService, IUserService userService, IMovieListRepository movieListRepository)
        {
            _movieService = movieService;
            _userService = userService;
            _movieListRepository = movieListRepository;
        }

        public async Task<MovieListResponseDTO> AddMovieToList(int movieId, int userId)
        {
            var movie = await _movieService.GetMovieById(movieId);

            var user = await _userService.GetUserById(userId);

            if (movie == null) throw new ResourceNotFoundException("Movie not found");
            MovieList movieList = new()
            {
                MovieId = movieId,
                UserId = userId
            };


            await _movieListRepository.CreateMovieList(movieList);

            return MovieListMapper.EntityToResponse(movieList, user.Name);
        }

        public async Task<PagedResult<MovieDetailsResponseDTO>> GetMovieList(PaginationParams paginationParams, int userId)
        {
            var allMovieList = await _movieListRepository.ListMoviesByUserId(userId);

            var query = allMovieList.AsQueryable();

            var totalItems = query.Count();

            var pagedMovies = query.OrderBy(ml => ml.MovieId).Skip((paginationParams.PageNumber - 1) * paginationParams.PageSize).Take(paginationParams.PageSize).ToList();

            var movies = new List<MovieDetailsResponseDTO>();
            foreach (var movie in pagedMovies)
            {
                var movieDetails = await _movieService.GetMovieById(movie.MovieId);
                movies.Add(movieDetails);
            }

            return new PagedResult<MovieDetailsResponseDTO>
            {
                Items = movies,
                TotalItems = totalItems,
                PageNumber = paginationParams.PageNumber,
                PageSize = paginationParams.PageSize
            };
        }

        public async Task<bool> RemoveMovieFromList(int id, int userId)
        {
            var movieList = await _movieListRepository.FindMovieListById(id);

            if (movieList == null) throw new ResourceNotFoundException("Movie not found.");
            if (movieList.UserId != userId) throw new ForbiddenException("You can't remove a movie that is not in your list");

            await _movieListRepository.RemoveMovieFromList(movieList);

            return true;
        }
    }
}
