using Domain.Enums;
using Domain.Models;
using Domain.Models.Pagination;

namespace Domain.Interface.Repositories
{
    public interface IMovieRepository
    {
        public Task<PagedResult<Movie>> GetAllMovies(PaginationParams paginationParams, 
            string? title, 
            string? director, 
            string? genre, 
            string? actor, 
            MovieOrderBy order, 
            CancellationToken cancellationToken = default);
        public Task<List<Movie>> GetMoviesByIds(List<int> movieIds, CancellationToken cancellationToken = default);
        public Task<Movie> CreateMovie(Movie movie, CancellationToken cancellationToken = default);
        public void DeleteMovie(Movie movie);
        public void UpdateRating(Movie movie);
        public Task<Movie?> FindMovieById(int id, CancellationToken cancellationToken = default);
        public Task<bool> FindMovieByTitle(string title, CancellationToken cancellationToken = default);
    }
}
