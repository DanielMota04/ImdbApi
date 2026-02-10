namespace Domain.Models
{
    public class MovieList
    {
        public int MovieListId { get; set; }
        public int UserId { get; set; }
        public int MovieId { get; set; }
        public bool IsVoted { get; set; } = false;
    }
}
