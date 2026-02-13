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

        public async Task<MovieList> CreateMovieList(MovieList movieList)
        {
            _context.MovieLists.Add(movieList);
            await _context.SaveChangesAsync();
            return movieList;
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

        public async Task<IEnumerable<MovieList>> ListMoviesByUserId(int userId)
        {
            return await _context.MovieLists.Where(ml => ml.UserId == userId).AsNoTracking().ToListAsync();
        }

        public async void RemoveMovieFromList(MovieList movieList)
        {
            _context.MovieLists.Remove(movieList);
            await _context.SaveChangesAsync();
        }

        public async void UpdateIsVoted(MovieList movieList)
        {
            movieList.IsVoted = true;
            _context.MovieLists.Update(movieList);
            await _context.SaveChangesAsync();
        }
    }
}
