namespace Application.DTOs.Response.Movie
{
    public class MovieDetailsResponseDTO
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public required double Rating { get; set; }
        public required string Genre { get; set; }
        public required string Director { get; set; }
        public required List<string> Actors { get; set; }
    }
}