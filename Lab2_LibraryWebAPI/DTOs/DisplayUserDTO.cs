namespace Lab2_LibraryWebAPI.DTOs
{
    public class DisplayUserDTO
    {
        public int CardNumber { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public string? Email { get; set; }
    }
}
