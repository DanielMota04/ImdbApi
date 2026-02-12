using Domain.Models;

namespace Domain.Interface.Repositories
{
    public interface IMovieListRepository
    {
        public Task<MovieList> CreateMovieList(MovieList ml);
        public Task<IEnumerable<MovieList>> ListMoviesByUserId(int id);
        public Task<MovieList?> FindMovieListById(int id);
        public Task<bool> RemoveMovieFromList(MovieList ml);
        public Task<bool> IsMovieOnUserList(int userId);
        public Task<bool> UpdateIsVoted(MovieList ml);
        public Task<MovieList?> FindMovieInListByMovieIdAndUserId(int movieId, int userId);
    }
}
