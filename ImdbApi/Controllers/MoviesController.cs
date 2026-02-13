using Api.Extensions;
using Application.DTOs.Request.Movie;
using Application.DTOs.Response.Movie;
using Application.Interfaces;
using Domain.Enums;
using Domain.Models;
using Domain.Models.Pagination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : BaseApiController
    {
        private readonly IMovieService _service;
        private readonly IMovieListService _movieListService;

        public MoviesController(IMovieService service, IMovieListService movieListService)
        {
            _service = service;
            _movieListService = movieListService;
        }

        [HttpGet]
        public async Task<IActionResult> GetMovies(
            [FromQuery] PaginationParams paginationParams,
            [FromQuery] string? title, string? director, string? genre, string? actors, MovieOrderBy order)
        {
            var movies = await _service.GetAllMovies(paginationParams, title, director, genre, actors, order);
            return HandleResult(movies);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMovie(int id)
        {
            var movie = await _service.GetMovieById(id);
            return HandleResult(movie);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateMovie(CreateMovieRequestDTO dto)
        {
            var createdMovie = await _service.CreateMovie(dto);
            return HandleResult(createdMovie);
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
            var userId = User.GetUserId();
            var result = await _movieListService.AddMovieToList(id, userId);
            return HandleResult(result);
        }

        [Authorize]
        [HttpGet("myList")]
        public async Task<IActionResult> GetMovieList([FromQuery] PaginationParams paginationParams)
        {
            var userId = User.GetUserId();
            var result = await _movieListService.GetMovieList(paginationParams, userId);
            return HandleResult(result);
        }

        [Authorize]
        [HttpDelete("{id}/removeFromList")]
        public async Task<IActionResult> RemoveMovieFromList(int id)
        {
            var userId = User.GetUserId();
            var result = await _movieListService.RemoveMovieFromList(id, userId);
            return HandleResult(result);
        }


        [Authorize]
        [HttpPut("vote")]
        public async Task<IActionResult> Vote(VoteMovieRequestDTO dto)
        {
            var userId = User.GetUserId();
            var value = await _service.Vote(dto, userId);
            return HandleResult(value);
        }

    }
}
