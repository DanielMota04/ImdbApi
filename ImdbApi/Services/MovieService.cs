using ImdbApi.Data;
using ImdbApi.DTOs.Request;
using ImdbApi.DTOs.Response;
using ImdbApi.Interfaces.Repositories;
using ImdbApi.Interfaces.Services;
using ImdbApi.Mappers;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace ImdbApi.Services
{
    public class MovieService : IMovieService
    {
        private readonly MovieMapper _mapper;
        private readonly IMovieRepository _repository;

        public MovieService(MovieMapper mapper, IMovieRepository repository)
        {
            _mapper = mapper;
            _repository = repository;
        }

        public async Task<MovieDetailsResponseDTO> CreateMovie(CreateMovieRequestDTO dto)
        {
            var title = dto.Title.Trim().Normalize();
            var genre = dto.Genre.Trim().Normalize();
            var director = dto.Director.Trim().Normalize();

            if (await _repository.FindMovieByTitle(title)) return null;

            var movie = _mapper.CreateToEntity(title, genre, director);
            await _repository.CreateMovie(movie);

            return _mapper.EntityToDetails(movie);
        }

        public async Task<IEnumerable<MovieResponseDTO>> GetAllMovies()
        {
            var movies = await _repository.GetAllMovies();
            var moviesDTO = movies.Select(m => _mapper.EntityToResponse(m));

            return moviesDTO;
        }

        public async Task<MovieDetailsResponseDTO> GetMovieById(int id)
        {
            var movie = await _repository.FindMovieById(id);
            if (movie == null) return null;
            return _mapper.EntityToDetails(movie);
        }

        public async Task<bool> DeleteMovie(int id)
        {
            var movie = await _repository.FindMovieById(id);

            if (movie == null) return false;
            await _repository.DeleteMovie(movie);

            return true;
        }
    }
}
