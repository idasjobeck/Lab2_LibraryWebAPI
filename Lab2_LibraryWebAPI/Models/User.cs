namespace Lab2_LibraryWebAPI.Models
{
    public class User
    {
        public int Id { get; set; }
        public int CardNumber { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public string? Email { get; set; }

        public List<Loan> Loans { get; set; }
    }
}
