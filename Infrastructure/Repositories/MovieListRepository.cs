using Domain.Interface.Repositories;
using Domain.Models;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class MovieListRepository : IMovieListRepository
    {
        private readonly AppDbContext _context;

        public MovieListRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<MovieList> CreateMovieList(MovieList ml)
        {
            _context.MovieLists.Add(ml);
            await _context.SaveChangesAsync();
            return ml;
        }

        public async Task<MovieList?> FindMovieInListByMovieIdAndUserId(int movieId, int userId)
        {
            return await _context.MovieLists.FirstOrDefaultAsync(ml => ml.UserId == userId && ml.MovieId == movieId);
        }

        public async Task<MovieList?> FindMovieListById(int id)
        {
            return await _context.MovieLists.FirstOrDefaultAsync(ml => ml.MovieListId == id);
        }

        public async Task<bool> IsMovieOnUserList(int userId)
        {
            return await _context.MovieLists.AnyAsync(ml => ml.UserId == userId);
        }

        public async Task<IEnumerable<MovieList>> ListMoviesByUserId(int id)
        {
            return await _context.MovieLists.Where(ml => ml.UserId == id).ToListAsync();
        }

        public async Task<bool> RemoveMovieFromList(MovieList ml)
        {
            _context.MovieLists.Remove(ml);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateIsVoted(MovieList ml)
        {
            MovieList movielist = ml;
            movielist.IsVoted = true;
            _context.MovieLists.Update(movielist);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
