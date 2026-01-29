using ImdbApi.DTOs.Response;

namespace ImdbApi.Interfaces
{
    public interface IMovieListService
    {
        public Task<MovieListResponseDTO> AddMovieToList(int movieId, string username);
        public Task<bool> RemoveMovieFronList(int id);
    }
}
