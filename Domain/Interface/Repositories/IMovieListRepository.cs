using Domain.Models;
using Domain.Models.Pagination;

namespace Domain.Interface.Repositories
{
    public interface IMovieListRepository
    {
        public Task<PagedResult<MovieList>> GetAllMoviesInList(PaginationParams paginationParams, int userId, CancellationToken cancellationToken = default);
        public Task<MovieList?> FindMovieListById(int id, CancellationToken cancellationToken = default);
        public Task<MovieList> CreateMovieList(MovieList movieList, CancellationToken cancellationToken = default);
        public void RemoveMovieFromList(MovieList movieList);
        public void UpdateIsVoted(MovieList movieList);
        public Task<bool> IsMovieOnUserList(int userId, CancellationToken cancellationToken = default);
        public Task<MovieList?> FindMovieInListByMovieIdAndUserId(int movieId, int userId, CancellationToken cancellationToken = default);
    }
}
