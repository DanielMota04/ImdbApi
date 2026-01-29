using ImdbApi.Data;
using ImdbApi.DTOs.Request;
using ImdbApi.DTOs.Response;
using ImdbApi.Interfaces;
using ImdbApi.Mappers;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace ImdbApi.Services
{
    public class MovieService : IMovieService
    {
        private readonly AppDbContext _context;
        private readonly MovieMapper _mapper;

        public MovieService(AppDbContext context, MovieMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<MovieDetailsResponseDTO> CreateMovie(CreateMovieRequestDTO dto)
        {
            var title = dto.Title.Trim().Normalize();
            var genre = dto.Genre.Trim().Normalize();
            var director = dto.Director.Trim().Normalize();

            if (await _context.Movies.AnyAsync(m => m.Title == title)) return null;

            var movie = _mapper.CreateToEntity(title, genre, director);
            _context.Movies.Add(movie);
            await _context.SaveChangesAsync();

            return _mapper.EntityToDetails(movie);
        }

        public async Task<IEnumerable<MovieResponseDTO>> GetAllMovies()
        {
            var movies = await _context.Movies.ToListAsync();
            var moviesDTO = movies.Select(m => _mapper.EntityToResponse(m));

            return moviesDTO;
        }

        public async Task<MovieDetailsResponseDTO> GetMovieById(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie == null) return null;
            return _mapper.EntityToDetails(movie);
        }

        public async Task<bool> DeleteMovie(int id)
        {
            var movie = await _context.Movies.FindAsync(id);

            if (movie == null) return false;
            _context.Movies.Remove(movie);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
