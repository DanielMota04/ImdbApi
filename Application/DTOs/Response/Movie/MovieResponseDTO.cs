namespace Application.DTOs.Response.Movie
{
    public class MovieResponseDTO
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public double Rating { get; set; }
    }
}
