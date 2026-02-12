namespace Application.DTOs.Response.Movie
{
    public class MovieListResponseDTO
    {
        public int MovieListId { get; set; }
        public int MovieId { get; set; }
        public required string Username { get; set; }
    }
}
