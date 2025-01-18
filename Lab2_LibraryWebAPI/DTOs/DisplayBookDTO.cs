namespace Lab2_LibraryWebAPI.DTOs
{
    public class DisplayBookDTO
    {
        public TitleNameDTO Title { get; set; }
        public SeriesNameDTO? Series { get; set; }
        public int? NumberInSeries { get; set; }
        public List<AuthorNameDTO> Authors { get; set; }
        public GenreNameDTO Genre { get; set; }
        public string ISBN { get; set; }
        public int PublishedYear { get; set; }
        public PublisherNameDTO Publisher { get; set; }
        public EditionNameDTO Edition { get; set; }
        public int TotalQty { get; set; }
        public int AvailableQty { get; set; }
    }
}
