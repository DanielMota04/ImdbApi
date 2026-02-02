using ImdbApi.DTOs.Request;
using ImdbApi.DTOs.Response;

namespace ImdbApi.Interfaces.Services
{
    public interface IMovieService
    {
        public Task<MovieDetailsResponseDTO> GetMovieById(int id);
        public Task<IEnumerable<MovieResponseDTO>> GetAllMovies();
        public Task<MovieDetailsResponseDTO> CreateMovie(CreateMovieRequestDTO dto);
        public Task<bool> DeleteMovie(int id);
    }
}
