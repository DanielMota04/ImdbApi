using ImdbApi.Models;

namespace ImdbApi.Interfaces.Repositories
{
    public interface IMovieListRepository
    {
        public Task<MovieList> CreateMovieList(MovieList ml);
        public Task<IEnumerable<MovieList>> ListMoviesByUserId(int id);
        public Task<MovieList> FindMovieListById(int id);
        public Task<bool> RemoveMovieFromList(MovieList ml);
        public Task<bool> IsMovieOnUserList(int userId);
    }
}
