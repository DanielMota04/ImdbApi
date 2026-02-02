using ImdbApi.DTOs.Request.Movie;
using ImdbApi.DTOs.Response.Movie;
using ImdbApi.Enums;
using ImdbApi.Interfaces.Repositories;
using ImdbApi.Interfaces.Services;
using ImdbApi.Mappers;
using ImdbApi.Repositories;
using System.IO;
using System.Runtime.Intrinsics.Arm;
using System.Security.Claims;

namespace ImdbApi.Services
{
    public class MovieService : IMovieService
    {
        private readonly MovieMapper _mapper;
        private readonly IMovieRepository _movieRepository; 
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMovieListRepository _movieListRepository;

        public MovieService(MovieMapper mapper, IMovieRepository movieRepository, IHttpContextAccessor httpContextAccessor, IMovieListRepository movieListRepository)
        {
            _mapper = mapper;
            _movieRepository = movieRepository;
            _httpContextAccessor = httpContextAccessor;
            _movieListRepository = movieListRepository;
        }

        public async Task<MovieDetailsResponseDTO> CreateMovie(CreateMovieRequestDTO dto)
        {
            var title = dto.Title.Trim().Normalize();
            var genre = dto.Genre.Trim().Normalize();
            var director = dto.Director.Trim().Normalize();

            if (await _movieRepository.FindMovieByTitle(title)) return null;

            var movie = _mapper.CreateToEntity(title, genre, director); 
            await _movieRepository.CreateMovie(movie);

            return _mapper.EntityToDetails(movie);
        }

        public async Task<IEnumerable<MovieResponseDTO>> GetAllMovies(string? title, string? director, string? genre, List<string>? actors, MovieOrderBy order)
        {
            var movies = await _movieRepository.GetAllMovies();

            if (title != null)
            {
                movies = movies.Where(m => m.Title.Contains(title));
            }
            if (director != null)
            {
                movies = movies.Where(m => m.Director.Contains(director));
            }
            if (genre != null)
            {
                movies = movies.Where(m => m.Genre.Contains(genre));
            }

            if (order.ToString().Equals("Rating"))
            {
                movies = movies.OrderByDescending(m => m.Rating).ToList();
            }
            else if (order.ToString().Equals("Alphabetic"))
            {
                movies = movies.OrderBy(m => m.Title).ToList();
            }

            var moviesDTO = movies.Select(m => _mapper.EntityToResponse(m));

            return moviesDTO;
        }

        public async Task<MovieDetailsResponseDTO> GetMovieById(int id)
        {
            var movie = await _movieRepository.FindMovieById(id);
            if (movie == null) return null;
            return _mapper.EntityToDetails(movie);
        }

        public async Task<bool> DeleteMovie(int id)
        {
            var movie = await _movieRepository.FindMovieById(id);

            if (movie == null) return false;
            await _movieRepository.DeleteMovie(movie);

            return true;
        }

        public async Task<double?> Vote(int movieId, double vote)
        {
            var movie = await _movieRepository.FindMovieById(movieId);
            var userId = int.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var movieList = await _movieListRepository.ListMoviesByUserId(userId);

            if (!await _movieListRepository.IsMovieOnUserList(userId)) return null;

            movie.TotalRating += vote;
            movie.Votes += 1;
            movie.Rating = movie.TotalRating / movie.Votes;

            await _movieRepository.UpdateRating(movie);

            return movie.Rating;
        }
    }
}
