using Application.DTOs.Response.Movie;
using Application.Interfaces;
using Application.Mappers;
using Domain.Interface.Repositories;
using Domain.Models;
using Domain.Exceptions;
using Domain.Models.Pagination;

namespace Application.Services
{
    public class MovieListService : IMovieListService
    {
        private readonly IMovieService _movieService;
        private readonly IMovieRepository _movieRepository;
        private readonly IUserService _userService;
        private readonly IMovieListRepository _movieListRepository;

        public MovieListService(IMovieService movieService, IUserService userService, IMovieListRepository movieListRepository, IMovieRepository movieRepository)
        {
            _movieService = movieService;
            _userService = userService;
            _movieListRepository = movieListRepository;
            _movieRepository = movieRepository;
        }
        
        public async Task<PagedResult<MovieDetailsResponseDTO>> GetMovieList(PaginationParams paginationParams, int userId)
        {
            var pagedList = await _movieListRepository.ListMoviesByUserId(paginationParams, userId);

            if (pagedList.Items == null || !pagedList.Items.Any())
            {
                return new PagedResult<MovieDetailsResponseDTO>
                {
                    Items = new List<MovieDetailsResponseDTO>(),
                    TotalItems = 0,
                    PageNumber = paginationParams.PageNumber,
                    PageSize = paginationParams.PageSize
                };
            }

            var movieIds = pagedList.Items.Select(ml => ml.MovieId).ToList();

            var moviesDetails = await _movieRepository.GetMoviesByIds(movieIds);

            var mappedItems = pagedList.Items
                .Select(ml => {
                    var movie = moviesDetails.FirstOrDefault(m => m.Id == ml.MovieId);

                    return movie != null ? MovieMapper.EntityToDetails(movie) : null;
                })
                .Where(x => x != null)
                .Cast<MovieDetailsResponseDTO>()
                .ToList();

            return new PagedResult<MovieDetailsResponseDTO>
            {
                Items = mappedItems,
                TotalItems = pagedList.TotalItems,
                PageNumber = pagedList.PageNumber,
                PageSize = pagedList.PageSize
            };
        }

        public async Task<MovieListResponseDTO> AddMovieToList(int movieId, int userId)
        {
            var movie = await _movieService.GetMovieById(movieId);

            var user = await _userService.GetUserById(userId);

            if (movie == null)
                throw new ResourceNotFoundException("Movie not found");

            if (user == null)
                throw new ResourceNotFoundException("User not found");

            MovieList movieList = new()
            {
                MovieId = movieId,
                UserId = userId
            };


            await _movieListRepository.CreateMovieList(movieList);

            return MovieListMapper.EntityToResponse(movieList, user.Name);
        }

        public async Task<bool> RemoveMovieFromList(int id, int userId)
        {
            var movieList = await _movieListRepository.FindMovieListById(id);

            if (movieList == null) 
                throw new ResourceNotFoundException("Movie not found.");
            
            if (movieList.UserId != userId) 
                throw new ForbiddenException("You can't remove a movie that is not in your list");

            _movieListRepository.RemoveMovieFromList(movieList);

            return true;
        }
    }
}
