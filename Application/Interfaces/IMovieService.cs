using Application.DTOs.Request.Movie;
using Application.DTOs.Response.Movie;
using Domain.Enums;
using Domain.Models.Pagination;

namespace Application.Interfaces
{
    public interface IMovieService
    {
        public Task<MovieDetailsResponseDTO> GetMovieById(int id);
        public Task<PagedResult<MovieResponseDTO>> GetAllMovies(PaginationParams paginationParams, string? title, string? director, string? genre, string? actors, MovieOrderBy order);
        public Task<MovieDetailsResponseDTO> CreateMovie(CreateMovieRequestDTO dto);
        public Task<bool> DeleteMovie(int id);
        public Task<double?> Vote(VoteMovieRequestDTO vote, int userId);
    }
}
