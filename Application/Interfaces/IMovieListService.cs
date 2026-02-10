using Application.DTOs.Pagination;
using Application.DTOs.Response.Movie;

namespace Application.Interfaces
{
    public interface IMovieListService
    {
        public Task<MovieListResponseDTO> AddMovieToList(int movieId);
        public Task<bool> RemoveMovieFromList(int id);
        public Task<PagedResult<MovieDetailsResponseDTO>> GetMovieList(PaginationParams paginationParams);
    }
}
