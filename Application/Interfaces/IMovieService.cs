using Application.DTOs.Request.Movie;
using Application.DTOs.Response.Movie;
using Domain.Enums;
using Domain.Models.Pagination;
using FluentResults;

namespace Application.Interfaces
{
    public interface IMovieService
    {
        public Task<Result<MovieDetailsResponseDTO>> GetMovieById(int id);
        public Task<Result<PagedResult<MovieResponseDTO>>> GetAllMovies(PaginationParams paginationParams, string? title, string? director, string? genre, string? actors, MovieOrderBy order);
        public Task<Result<MovieDetailsResponseDTO>> CreateMovie(CreateMovieRequestDTO dto);
        public Task<Result<bool>> DeleteMovie(int id);
        public Task<Result<double>> Vote(VoteMovieRequestDTO vote, int userId);
    }
}
