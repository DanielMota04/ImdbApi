using Domain.Interface.Repositories;
using Domain.Models;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
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

        public async Task<bool> UpdateRating(Movie m)
        {
            var movie = _context.Movies.Update(m);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
