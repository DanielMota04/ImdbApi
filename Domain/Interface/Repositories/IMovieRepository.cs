using Domain.Models;

namespace Domain.Interface.Repositories
{
    public interface IMovieRepository
    {
        public Task<IEnumerable<Movie>> GetAllMovies();
        public Task<Movie> CreateMovie(Movie m);
        public void DeleteMovie(Movie m);
        public void UpdateRating(Movie m);
        public Task<Movie?> FindMovieById(int id);
        public Task<bool> FindMovieByTitle(string title);
    }
}
