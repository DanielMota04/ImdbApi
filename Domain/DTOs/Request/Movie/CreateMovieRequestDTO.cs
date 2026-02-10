namespace ImdbApi.DTOs.Request.Movie
{
    public class CreateMovieRequestDTO
    {
        public required string Title { get; set; }
        public required string Genre { get; set; }
        public required string Director { get; set; }
        public required List<string> Actors { get; set; }
    }
}
