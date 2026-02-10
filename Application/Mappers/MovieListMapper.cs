using Application.DTOs.Response.Movie;
using Domain.Models;

namespace Application.Mappers
{
    public class MovieListMapper
    {
        public MovieListResponseDTO EntityToResponse(MovieList entity, string username)
        {
            MovieListResponseDTO dto = new()
            {
                MovieListId = entity.MovieListId,
                MovieId = entity.MovieId,
                Username = username
            };

            return dto;
        }
    }
}
