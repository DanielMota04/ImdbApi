namespace Domain.Models
{
    public class Movie
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public required double Rating { get; set; } = 0.0;
        public double TotalRating { get; set; } = 0.0;
        public int Votes { get; set; } = 0;
        public required string Genre { get; set; }
        public required string Director { get; set; }
        public required List<string> Actors { get; set; }
    }
}
