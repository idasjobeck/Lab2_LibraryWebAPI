namespace Lab2_LibraryWebAPI.DTOs
{
    public class CreateBookWithIdsNewTitleDTO
    {
        public required string Title { get; set; }
        public string? Series { get; set; }
        public int? NumberInSeries { get; set; }
        public required List<int> AuthorIds { get; set; }
        public int GenreId { get; set; }
        public required string ISBN { get; set; }
        public int PublishedYear { get; set; }
        public int PublisherId { get; set; }
        public int EditionId { get; set; }
        public int TotalQty { get; set; }
        public int AvailableQty { get; set; }
    }
}
