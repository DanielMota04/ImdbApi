using ImdbApi.DTOs.Response.Movie;
using ImdbApi.Models;

namespace ImdbApi.Mappers
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
