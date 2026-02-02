using ImdbApi.DTOs.Request;
using ImdbApi.DTOs.Response;

namespace ImdbApi.Interfaces.Services
{
    public interface IMovieService
    {
        public Task<MovieDetailsResponseDTO> GetMovieById(int id);
        public Task<IEnumerable<MovieResponseDTO>> GetAllMovies(string? title, string? director, string? genre); // futuro: adicionar atores
        public Task<MovieDetailsResponseDTO> CreateMovie(CreateMovieRequestDTO dto);
        public Task<bool> DeleteMovie(int id);
        public Task<double?> Vote(int movieId, double vote);
    }
}
