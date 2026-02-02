using ImdbApi.DTOs.Request.Movie;
using ImdbApi.DTOs.Response.Movie;
using ImdbApi.Enums;

namespace ImdbApi.Interfaces.Services
{
    public interface IMovieService
    {
        public Task<MovieDetailsResponseDTO> GetMovieById(int id);
        public Task<IEnumerable<MovieResponseDTO>> GetAllMovies(string? title, string? director, string? genre, List<string>? actors, MovieOrderBy order);
        public Task<MovieDetailsResponseDTO> CreateMovie(CreateMovieRequestDTO dto);
        public Task<bool> DeleteMovie(int id);
        public Task<double?> Vote(int movieId, double vote);
    }
}
