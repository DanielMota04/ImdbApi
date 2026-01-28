namespace ImdbApi.Models
{
    public class Movie
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public required double Rating { get; set; }
        public required string Genre { get; set; }
        public required string Director { get; set; }
        //public string Actors { get; set; }

    }
}
