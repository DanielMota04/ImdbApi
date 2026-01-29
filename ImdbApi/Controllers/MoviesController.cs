using Microsoft.AspNetCore.Mvc;
using ImdbApi.Models;
using ImdbApi.Interfaces;
using ImdbApi.DTOs.Request;

namespace ImdbApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly IMovieService _service;

        public MoviesController(IMovieService service)
        {
            _service = service;
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

    }
}
