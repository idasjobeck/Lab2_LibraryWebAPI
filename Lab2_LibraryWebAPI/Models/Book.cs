namespace Lab2_LibraryWebAPI.Models
{
    public class Book
    {
        public int Id { get; set; }
        public required Title Title { get; set; }
        public Series? Series { get; set; }
        public int? NumberInSeries { get; set; }
        public required Genre Genre { get; set; }
        public required string ISBN { get; set; }
        public DateOnly PublishedYear { get; set; }
        public required Publisher Publisher { get; set; }
        public required Edition Edition { get; set; }
        public int TotalQty { get; set; }
        public int AvailableQty { get; set; }

        public List<Author> Authors { get; set; } = new();
        public List<Loan> Loans { get; set; } = new();
    }
}
