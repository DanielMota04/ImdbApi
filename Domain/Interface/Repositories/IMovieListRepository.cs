using Domain.Models;
using Domain.Models.Pagination;

namespace Domain.Interface.Repositories
{
    public interface IMovieListRepository
    {
        public Task<PagedResult<MovieList>> ListMoviesByUserId(PaginationParams paginationParams, int userId);
        public Task<MovieList?> FindMovieListById(int id);
        public Task<MovieList> CreateMovieList(MovieList movieList);
        public void RemoveMovieFromList(MovieList movieList);
        public void UpdateIsVoted(MovieList movieList);
        public Task<bool> IsMovieOnUserList(int userId);
        public Task<MovieList?> FindMovieInListByMovieIdAndUserId(int movieId, int userId);
    }
}
