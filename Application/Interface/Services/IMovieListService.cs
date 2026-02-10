using ImdbApi.DTOs.Pagination;
using ImdbApi.DTOs.Response.Movie;

namespace ImdbApi.Interfaces.Services
{
    public interface IMovieListService
    {
        public Task<MovieListResponseDTO> AddMovieToList(int movieId);
        public Task<bool> RemoveMovieFromList(int id);
        public Task<PagedResult<MovieDetailsResponseDTO>> GetMovieList(PaginationParams paginationParams);
    }
}
