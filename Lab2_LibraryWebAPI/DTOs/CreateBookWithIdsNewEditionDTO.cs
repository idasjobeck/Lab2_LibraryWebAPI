namespace Lab2_LibraryWebAPI.DTOs
{
    public class CreateBookWithIdsNewEditionDTO
    {
        public int TitleId { get; set; }
        public int? SeriesId { get; set; }
        public int? NumberInSeries { get; set; }
        public List<int> AuthorIds { get; set; }
        public int GenreId { get; set; }
        public string ISBN { get; set; }
        public int PublishedYear { get; set; }
        public string Publisher { get; set; }
        public string Edition { get; set; }
        public int TotalQty { get; set; }
        public int AvailableQty { get; set; }
    }
}
