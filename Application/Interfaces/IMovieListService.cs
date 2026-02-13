using Application.DTOs.Response.Movie;
using Domain.Models.Pagination;

namespace Application.Interfaces
{
    public interface IMovieListService
    {
        public Task<MovieListResponseDTO> AddMovieToList(int movieId, int userId);
        public Task<bool> RemoveMovieFromList(int id, int userId);
        public Task<PagedResult<MovieDetailsResponseDTO>> GetMovieList(PaginationParams paginationParams, int userId);
    }
}
