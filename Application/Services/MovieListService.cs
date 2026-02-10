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
        private readonly MovieListMapper _mapper;
        private readonly IMovieService _movieService;
        private readonly IUserService _userService;
        private readonly IMovieListRepository _movieListRepository;
        private readonly IHttpContextAccessor _httpContextAcessor;

        public MovieListService(MovieListMapper mapper, IMovieService movieService, IUserService userService, IMovieListRepository movieListRepository, IHttpContextAccessor httpContextAcessor)
        {
            _mapper = mapper;
            _movieService = movieService;
            _userService = userService;
            _movieListRepository = movieListRepository;
            _httpContextAcessor = httpContextAcessor;
        }

        public async Task<MovieListResponseDTO> AddMovieToList(int movieId)
        {
            var movie = await _movieService.GetMovieById(movieId);
            var userId = int.Parse(_httpContextAcessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var user = await _userService.GetUserById(userId);

            if (movie == null) throw new ResourceNotFoundException("Movie not found");
            MovieList movieList = new()
            {
                MovieId = movieId,
                UserId = userId
            };


            await _movieListRepository.CreateMovieList(movieList);

            return _mapper.EntityToResponse(movieList, user.Name);
        }

        public async Task<PagedResult<MovieDetailsResponseDTO>> GetMovieList(PaginationParams paginationParams)
        {
            var userId = int.Parse(_httpContextAcessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
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

        public async Task<bool> RemoveMovieFromList(int id)
        {
            var movieList = await _movieListRepository.FindMovieListById(id);

            var userId = int.Parse(_httpContextAcessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            if (movieList == null) throw new ResourceNotFoundException("Movie not found.");
            if (movieList.UserId != userId) throw new ForbiddenException("You can't remove a movie that is not in your list");

            await _movieListRepository.RemoveMovieFromList(movieList);

            return true;
        }
    }
}
