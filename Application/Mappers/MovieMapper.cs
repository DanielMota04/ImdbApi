using Application.DTOs.Response.Movie;
using Domain.Models;

namespace Application.Mappers
{
    public class MovieMapper
    {
        public static Movie CreateToEntity(string title, string genre, List<string> actors, string director)
        {
            Movie movie = new()
            {
                Title = title,
                Rating = 0.0,
                Genre = genre,
                Actors = actors,
                Director = director
            };

            return movie;
        }

        public static MovieDetailsResponseDTO EntityToDetails(Movie movie)
        {
            MovieDetailsResponseDTO dto = new()
            {
                Id = movie.Id,
                Title = movie.Title,
                Rating = movie.Rating,
                Genre = movie.Genre,
                Director = movie.Director,
                Actors = movie.Actors,
            };

            return dto;
        }

        public static MovieResponseDTO EntityToResponse(Movie movie)
        {
            MovieResponseDTO dto = new()
            {
                Id = movie.Id,
                Title = movie.Title,
                Rating = movie.Rating
            };

            return dto;
        }
    }
}
