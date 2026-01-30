using Microsoft.AspNetCore.Mvc;
using ImdbApi.Models;
using ImdbApi.Interfaces;
using ImdbApi.DTOs.Request;
using Microsoft.AspNetCore.Authorization;
using ImdbApi.DTOs.Response;

namespace ImdbApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly IMovieService _service;
        private readonly IMovieListService _movieListService;

        public MoviesController(IMovieService service, IMovieListService movieListService)
        {
            _service = service;
            _movieListService = movieListService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Movie>>> GetMovies()
        {
            var movies = await _service.GetAllMovies();
            return Ok(movies);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Movie>> GetMovie(int id)
        {
            var movie = await _service.GetMovieById(id);
            if (movie == null) return NotFound();
            return Ok(movie);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateMovie(CreateMovieRequestDTO dto)
        {
            var createdMovie = await _service.CreateMovie(dto);
            if (createdMovie == null) return Conflict("A movie with the same title already exists.");
            return CreatedAtAction(nameof(GetMovie), new { id = createdMovie.Id }, createdMovie);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMovie(int id)
        {
            var result = await _service.DeleteMovie(id);
            if (!result) return NotFound();
            return NoContent();
        }


        [Authorize]
        [HttpPost("{id}/addToList")]
        public async Task<IActionResult> AddMovieToList(int id)
        {
            var result = await _movieListService.AddMovieToList(id);
            if (result == null) return NotFound("Movie not found.");
            return Ok(result);
        }

        [Authorize]
        [HttpGet("myList")]
        public async Task<ActionResult<IEnumerable<MovieDetailsResponseDTO>>> GetMovieList()
        {
            var result = await _movieListService.GetMovieList();
            return Ok(result);
        }

        [Authorize]
        [HttpDelete("{id}/removeFromList")]
        public async Task<IActionResult> RemoveMovieFromList(int id)
        {
            var result = await _movieListService.RemoveMovieFromList(id);
            if (!result) return NotFound("Movie not found in your list.");
            return Ok(result);
        }

    }
}
