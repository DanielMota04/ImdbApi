using Application.DTOs.Pagination;
using Application.DTOs.Request.Movie;
using Application.DTOs.Response.Movie;
using Application.Interfaces;
using Domain.Enums;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
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
        public async Task<ActionResult<IEnumerable<Movie>>> GetMovies(
            [FromQuery] PaginationParams paginationParams,
            [FromQuery] string? title, string? director, string? genre, string? actors, MovieOrderBy order)
        {
            var movies = await _service.GetAllMovies(paginationParams, title, director, genre, actors, order);
            return Ok(movies);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Movie>> GetMovie(int id)
        {
            var movie = await _service.GetMovieById(id);
            return Ok(movie);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateMovie(CreateMovieRequestDTO dto)
        {
            var createdMovie = await _service.CreateMovie(dto);
            return CreatedAtAction(nameof(GetMovie), new { id = createdMovie.Id }, createdMovie);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMovie(int id)
        {
            var result = await _service.DeleteMovie(id);
            return NoContent();
        }


        [Authorize]
        [HttpPost("{id}/addToList")]
        public async Task<IActionResult> AddMovieToList(int id)
        {
            var result = await _movieListService.AddMovieToList(id);
            return Ok(result);
        }

        [Authorize]
        [HttpGet("myList")]
        public async Task<ActionResult<IEnumerable<MovieDetailsResponseDTO>>> GetMovieList([FromQuery] PaginationParams paginationParams)
        {
            var result = await _movieListService.GetMovieList(paginationParams);
            return Ok(result);
        }

        [Authorize]
        [HttpDelete("{id}/removeFromList")]
        public async Task<IActionResult> RemoveMovieFromList(int id)
        {
            var result = await _movieListService.RemoveMovieFromList(id);
            return Ok(result);
        }


        [Authorize]
        [HttpPut("vote")]
        public async Task<ActionResult> Vote(VoteMovieRequestDTO dto)
        {
            var value = await _service.Vote(dto);
            return Ok(value);
        }

    }
}
