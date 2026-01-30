using ImdbApi.Data;
using ImdbApi.DTOs.Response;
using ImdbApi.Interfaces;
using ImdbApi.Interfaces.Repositories;
using ImdbApi.Mappers;
using ImdbApi.Models;
using System.Security.Claims;

namespace ImdbApi.Services
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

            if (movie == null) return null;
            MovieList movieList = new()
            {
                MovieId = movieId,
                UserId = userId
            };


            await _movieListRepository.CreateMovieList(movieList);

            return _mapper.EntityToResponse(movieList, user.Name);
        }

        public async Task<IEnumerable<MovieDetailsResponseDTO>> GetMovieList()
        {
            var userId = int.Parse(_httpContextAcessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var movieList = await _movieListRepository.ListMoviesByUserId(userId);

            var movies = new List<MovieDetailsResponseDTO>();
            foreach (var movie in movieList)
            {
                var movieDetails = await _movieService.GetMovieById(movie.MovieId);
                movies.Add(movieDetails);
            }

            return movies;
        }

        public async Task<bool> RemoveMovieFromList(int id)
        {
            var movieList = await _movieListRepository.FindMovieListById(id);

            var userId = int.Parse(_httpContextAcessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            if (movieList == null || movieList.UserId != userId) return false;

            await _movieListRepository.RemoveMovieFromList(movieList);

            return true;
        }
    }
}
