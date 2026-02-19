using Domain.Interface.Repositories;
using Domain.Models;
using Domain.Models.Pagination;
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

        public async Task<PagedResult<MovieList>> GetAllMoviesInList(PaginationParams paginationParams, int userId, CancellationToken cancellationToken)
        {
            var query = _context.MovieLists.AsQueryable().Where(ml => ml.UserId == userId);

            var totalItems = await query.CountAsync();

            var items = await query
                .OrderBy(ml => ml.MovieId)
                .Skip((paginationParams.PageNumber - 1) * paginationParams.PageSize)
                .Take(paginationParams.PageSize)
                .ToListAsync();

            return new PagedResult<MovieList>
            {
                Items = items,
                TotalItems = totalItems,
                PageNumber = paginationParams.PageNumber,
                PageSize = paginationParams.PageSize
            };
        }

        public async Task<MovieList> CreateMovieList(MovieList movieList, CancellationToken cancellationToken)
        {
            _context.MovieLists.Add(movieList);
            await _context.SaveChangesAsync();
            return movieList;
        }

        public async Task<MovieList?> FindMovieInListByMovieIdAndUserId(int movieId, int userId, CancellationToken cancellationToken)
        {
            return await _context.MovieLists.FirstOrDefaultAsync(ml => ml.UserId == userId && ml.MovieId == movieId);
        }

        public async Task<MovieList?> FindMovieListById(int id, CancellationToken cancellationToken)
        {
            return await _context.MovieLists.FirstOrDefaultAsync(ml => ml.MovieListId == id);
        }

        public async Task<bool> IsMovieOnUserList(int userId, CancellationToken cancellationToken)
        {
            return await _context.MovieLists.AnyAsync(ml => ml.UserId == userId);
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
