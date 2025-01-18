using Lab2_LibraryWebAPI.Models;

namespace Lab2_LibraryWebAPI.DTOs
{
    public class DisplayBookWithIdsDTO
    {
        public Title Title { get; set; }
        public Series? Series { get; set; }
        public int? NumberInSeries { get; set; }
        public List<AuthorNameWithIdDTO> Authors { get; set; }
        public Genre Genre { get; set; }
        public string ISBN { get; set; }
        public int PublishedYear { get; set; }
        public Publisher Publisher { get; set; }
        public Edition Edition { get; set; }
        public int TotalQty { get; set; }
        public int AvailableQty { get; set; }
    }
}
