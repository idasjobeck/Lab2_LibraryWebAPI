namespace Lab2_LibraryWebAPI.DTOs
{
    public class CreateLoanDTO
    {
        public required string Title { get; set; }
        public int CardNumber { get; set; }
    }
}
