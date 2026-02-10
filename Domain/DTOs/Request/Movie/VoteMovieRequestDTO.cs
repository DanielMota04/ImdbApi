namespace ImdbApi.DTOs.Request.Movie
{
    public class VoteMovieRequestDTO
    {
        public int MovieId { get; set; }
        public double Vote { get; set; }
    }
}
