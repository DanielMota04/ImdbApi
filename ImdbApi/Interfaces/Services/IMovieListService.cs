using ImdbApi.DTOs.Response;

namespace ImdbApi.Interfaces.Services
{
    public interface IMovieListService
    {
        public Task<MovieListResponseDTO> AddMovieToList(int movieId);
        public Task<bool> RemoveMovieFromList(int id);
        public Task<IEnumerable<MovieDetailsResponseDTO>> GetMovieList();
    }
}
