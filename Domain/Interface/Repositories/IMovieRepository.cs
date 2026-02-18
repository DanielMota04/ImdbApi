using Domain.Enums;
using Domain.Models;
using Domain.Models.Pagination;

namespace Domain.Interface.Repositories
{
    public interface IMovieRepository
    {
        public Task<PagedResult<Movie>> GetAllMovies(PaginationParams paginationParams, string? title, string? director, string? genre, string? actor, MovieOrderBy order);
        public Task<List<Movie>> GetMoviesByIds(List<int> movieIds);
        public Task<Movie> CreateMovie(Movie m);
        public void DeleteMovie(Movie m);
        public void UpdateRating(Movie m);
        public Task<Movie?> FindMovieById(int id);
        public Task<bool> FindMovieByTitle(string title);
    }
}
