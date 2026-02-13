using Domain.Enums;
using Domain.Interface.Repositories;
using Domain.Models;
using Domain.Models.Pagination;
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
        public async Task<PagedResult<Movie>> GetAllMovies(PaginationParams paginationParams, string? title, string? director, string? genre, string? actor, MovieOrderBy order)
        {
            var query = _context.Movies.AsQueryable();

            if (!string.IsNullOrWhiteSpace(title))
                query = query.Where(m => m.Title.Contains(title));
            
            if (!string.IsNullOrWhiteSpace(director))
                query = query.Where(m => m.Director.Contains(director));
            
            if (!string.IsNullOrWhiteSpace(genre))
                query = query.Where(m => m.Genre.Contains(genre));

            if (!string.IsNullOrWhiteSpace(actor))
                query = query.Where(m => m.Actors.Contains(actor));

            query = order switch
            {
                MovieOrderBy.Alphabetic => query.OrderBy(m => m.Title),
                MovieOrderBy.Rating => query.OrderByDescending(m => m.Rating),
                _ => query
            };

            var totalItems = await query.CountAsync();

            var items = await query
                .Skip((paginationParams.PageNumber - 1) * paginationParams.PageSize)
                .Take(paginationParams.PageSize)
                .ToListAsync();

            return new PagedResult<Movie>
            {
                Items = items,
                TotalItems = totalItems,
                PageNumber = paginationParams.PageNumber,
                PageSize = paginationParams.PageSize
            };
        }
        
        public async Task<List<Movie>> GetMoviesByIds(List<int> movieIds)
        {
            return await _context.Movies
                .Where(m => movieIds.Contains(m.Id))
                .ToListAsync();
        }

        public async Task<Movie> CreateMovie(Movie movie)
        {
            _context.Movies.Add(movie);
            await _context.SaveChangesAsync();
            return movie;
        }

        public async void DeleteMovie(Movie movie)
        {
            _context.Movies.Remove(movie);
            await _context.SaveChangesAsync();
        }

        public async Task<Movie?> FindMovieById(int id)
        {
            return await _context.Movies.FindAsync(id);
        }

        public async Task<bool> FindMovieByTitle(string title)
        {
            return await _context.Movies.AnyAsync(m => m.Title == title);
        }


        public async void UpdateRating(Movie movie)
        {
            _context.Movies.Update(movie);
            await _context.SaveChangesAsync();
        }
    }
}
