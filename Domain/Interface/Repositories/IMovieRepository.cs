using Domain.Models;

namespace Domain.Interface.Repositories
{
    public interface IMovieRepository
    {
        public Task<bool> FindMovieByTitle(string title);
        public Task<Movie> CreateMovie(Movie m);
        public Task<IEnumerable<Movie>> GetAllMovies();
        public Task<Movie?> FindMovieById(int id);
        public Task<bool> DeleteMovie(Movie m);
        public Task<bool> UpdateRating(Movie m);
    }
}
