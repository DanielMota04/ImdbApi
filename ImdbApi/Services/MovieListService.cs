using ImdbApi.Data;
using ImdbApi.DTOs.Response;
using ImdbApi.Interfaces;
using ImdbApi.Mappers;

namespace ImdbApi.Services
{
    public class MovieListService : IMovieListService
    {
        private readonly AppDbContext _context;
        private readonly MovieListMapper _mapper;
        private readonly IMovieService _movieService;
        // buscar por usuários

        public MovieListService(AppDbContext context, MovieListMapper mapper, IMovieService movieService)
        {
            _context = context;
            _mapper = mapper;
            _movieService = movieService;
        }

        public Task<MovieListResponseDTO> AddMovieToList(int movieId, string username)
        {
            throw new NotImplementedException();
        }

        //public Task<MovieListResponseDTO> AddMovieToList(int movieId, string username)
        //{
        //    var movie = _movieService.GetMovieById(movieId);
        //    if (movie == null) return null;

        // buscar por usuário
        //}

        public Task<bool> RemoveMovieFronList(int id)
        {
            throw new NotImplementedException();
        }
    }
}
