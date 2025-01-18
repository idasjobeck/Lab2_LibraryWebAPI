namespace Lab2_LibraryWebAPI.DTOs
{
    public class BookWithoutAuthorDTO
    {
        public required string Title { get; set; }
        public string? Series { get; set; }
        public int? NumberInSeries { get; set; }
        public string Genre { get; set; }
        public string ISBN { get; set; }
        public int PublishedYear { get; set; }
        public string Publisher { get; set; }
        public string Edition { get; set; }
        public int TotalQty { get; set; }
        public int AvailableQty { get; set; }
    }
}
