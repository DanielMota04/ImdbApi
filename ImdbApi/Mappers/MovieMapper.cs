using ImdbApi.DTOs.Response.Movie;
using ImdbApi.Models;

namespace ImdbApi.Mappers
{
    public class MovieMapper
    {
        public Movie CreateToEntity(string title, string genre, List<string> actors, string director)
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

        public MovieDetailsResponseDTO EntityToDetails(Movie movie)
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

        public MovieResponseDTO EntityToResponse(Movie movie)
        {
            MovieResponseDTO dto = new()
            {
                Id = movie.Id,
                Title = movie.Title,
                Rating = movie.Rating
            };

            return dto;
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public string Rating { get; set; }
    }
}
