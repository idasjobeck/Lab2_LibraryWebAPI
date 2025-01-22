using Lab2_LibraryWebAPI.Models;

namespace Lab2_LibraryWebAPI.DTOs
{
    public class DisplayBookWithIdsDTO
    {
        public required Title Title { get; set; }
        public required Series? Series { get; set; }
        public required int? NumberInSeries { get; set; }
        public required List<AuthorNameWithIdDTO> Authors { get; set; }
        public required Genre Genre { get; set; }
        public required string ISBN { get; set; }
        public int PublishedYear { get; set; }
        public required Publisher Publisher { get; set; }
        public required Edition Edition { get; set; }
        public int TotalQty { get; set; }
        public int AvailableQty { get; set; }
    }
}
