using Domain.Models;

namespace Domain.Interface.Repositories
{
    public interface IMovieListRepository
    {
        public Task<MovieList> CreateMovieList(MovieList movieList);
        public Task<IEnumerable<MovieList>> ListMoviesByUserId(int userId);
        public Task<MovieList?> FindMovieListById(int id);
        public void RemoveMovieFromList(MovieList movieList);
        public void UpdateIsVoted(MovieList movieList);
        public Task<bool> IsMovieOnUserList(int userId);
        public Task<MovieList?> FindMovieInListByMovieIdAndUserId(int movieId, int userId);
    }
}
