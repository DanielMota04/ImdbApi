using Application.DTOs.Pagination;
using Application.DTOs.Request.Movie;
using Application.DTOs.Response.Movie;
using Application.Interfaces;
using Domain.Enums;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly IMovieService _service;
        private readonly IMovieListService _movieListService;
        private readonly IHttpContextAccessor _httpContextAcessor;

        public MoviesController(IMovieService service, IMovieListService movieListService, IHttpContextAccessor httpContextAcessor)
        {
            _service = service;
            _movieListService = movieListService;
            _httpContextAcessor = httpContextAcessor;
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
            await _service.DeleteMovie(id);
            return NoContent();
        }


        [Authorize]
        [HttpPost("{id}/addToList")]
        public async Task<IActionResult> AddMovieToList(int id)
        {
            var userId = int.Parse(_httpContextAcessor.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _movieListService.AddMovieToList(id, userId);
            return Ok(result);
        }

        [Authorize]
        [HttpGet("myList")]
        public async Task<ActionResult<IEnumerable<MovieDetailsResponseDTO>>> GetMovieList([FromQuery] PaginationParams paginationParams)
        {
            var userId = int.Parse(_httpContextAcessor.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _movieListService.GetMovieList(paginationParams, userId);
            return Ok(result);
        }

        [Authorize]
        [HttpDelete("{id}/removeFromList")]
        public async Task<IActionResult> RemoveMovieFromList(int id)
        {
            var userId = int.Parse(_httpContextAcessor.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _movieListService.RemoveMovieFromList(id, userId);
            return Ok(result);
        }


        [Authorize]
        [HttpPut("vote")]
        public async Task<ActionResult> Vote(VoteMovieRequestDTO dto)
        {
            var userId = int.Parse(_httpContextAcessor.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var value = await _service.Vote(dto, userId);
            return Ok(value);
        }

    }
}
