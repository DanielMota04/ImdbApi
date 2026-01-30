using ImdbApi.Data;
using ImdbApi.Interfaces.Repositories;
using ImdbApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ImdbApi.Repositories
{
    public class MovieRepository : IMovieRepository
    {

        private readonly AppDbContext _context;

        public MovieRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Movie> CreateMovie(Movie m)
        {
            _context.Movies.Add(m);
            await _context.SaveChangesAsync();
            return m;
        }

        public async Task<bool> DeleteMovie(Movie m)
        {
            _context.Movies.Remove(m);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Movie?> FindMovieById(int id)
        {
            return await _context.Movies.FindAsync(id);
        }

        public async Task<bool> FindMovieByTitle(string title)
        {
            return await _context.Movies.AnyAsync(m => m.Title == title);
        }

        public async Task<IEnumerable<Movie>> GetAllMovies()
        {
            return await _context.Movies.ToListAsync();
        }
    }
}
