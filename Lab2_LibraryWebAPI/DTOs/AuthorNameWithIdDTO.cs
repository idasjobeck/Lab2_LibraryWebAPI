namespace Lab2_LibraryWebAPI.DTOs
{
    public class AuthorNameWithIdDTO
    {
        public int Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
    }
}
