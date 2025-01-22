namespace Lab2_LibraryWebAPI.DTOs
{
    public class CreateBookWithIdsNewEditionDTO
    {
        public int TitleId { get; set; }
        public int? SeriesId { get; set; }
        public int? NumberInSeries { get; set; }
        public required List<int> AuthorIds { get; set; }
        public int GenreId { get; set; }
        public required string ISBN { get; set; }
        public int PublishedYear { get; set; }
        public required string Publisher { get; set; }
        public required string Edition { get; set; }
        public int TotalQty { get; set; }
        public int AvailableQty { get; set; }
    }
}
