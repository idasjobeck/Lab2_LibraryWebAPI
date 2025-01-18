namespace Lab2_LibraryWebAPI.DTOs
{
    public class DisplayUserWithIdDTO
    {
        public int Id { get; set; }
        public int CardNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? Email { get; set; }
    }
}
