using Application.DTOs.Response.Movie;
using Domain.Models.Pagination;
using FluentResults;

namespace Application.Interfaces
{
    public interface IMovieListService
    {
        public Task<Result<MovieListResponseDTO>> AddMovieToList(int movieId, int userId);
        public Task<Result<bool>> RemoveMovieFromList(int id, int userId);
        public Task<Result<PagedResult<MovieDetailsResponseDTO>>> GetMovieList(PaginationParams paginationParams, int userId);
    }
}
