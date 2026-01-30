using ImdbApi.Data;
using ImdbApi.DTOs.Response;
using ImdbApi.Interfaces;
using ImdbApi.Mappers;
using ImdbApi.Models;
using System.Security.Claims;

namespace ImdbApi.Services
{
    public class MovieListService : IMovieListService
    {
        private readonly AppDbContext _context;
        private readonly MovieListMapper _mapper;
        private readonly IMovieService _movieService;
        private readonly IUserService _userService;
        private readonly IHttpContextAccessor _httpContextAcessor;

        public MovieListService(AppDbContext context, MovieListMapper mapper, IMovieService movieService, IUserService userService, IHttpContextAccessor httpContextAcessor)
        {
            _context = context;
            _mapper = mapper;
            _movieService = movieService;
            _userService = userService;
            _httpContextAcessor = httpContextAcessor;
        }

        public async Task<MovieListResponseDTO> AddMovieToList(int movieId)
        {
            var movie = await _movieService.GetMovieById(movieId);
            var userId = int.Parse(_httpContextAcessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var user = await _userService.GetUserById(userId);

            if (movie == null) return null;
            var movieList = new MovieList();
            movieList.MovieId = movieId;
            movieList.UserId = userId;
            
            _context.MovieLists.Add(movieList);
            await _context.SaveChangesAsync();

            return _mapper.EntityToResponse(movieList, user.Name);
        }

        public async Task<IEnumerable<MovieDetailsResponseDTO>> GetMovieList()
        {
            var userId = int.Parse(_httpContextAcessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var movieList = _context.MovieLists.Where(ml => ml.UserId == userId).ToList();

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
            var movieList = _context.MovieLists.FirstOrDefault(ml => ml.MovieListId == id);

            var userId = int.Parse(_httpContextAcessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            if (movieList == null || movieList.UserId != userId) return false;

            _context.MovieLists.Remove(movieList);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
