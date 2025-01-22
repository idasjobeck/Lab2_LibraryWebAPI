namespace Lab2_LibraryWebAPI.DTOs
{
    public class BookWithoutAuthorDTO
    {
        public required string Title { get; set; }
        public string? Series { get; set; }
        public int? NumberInSeries { get; set; }
        public required string Genre { get; set; }
        public required string ISBN { get; set; }
        public int PublishedYear { get; set; }
        public required string Publisher { get; set; }
        public required string Edition { get; set; }
        public int TotalQty { get; set; }
        public int AvailableQty { get; set; }
    }
}
