namespace Lab2_LibraryWebAPI.Models
{
    public class Loan
    {
        public int Id { get; set; }
        public Book Book { get; set; }
        public User User { get; set; }
        public string LoanDate { get; set; }
        public string? ReturnedDate { get; set; }
        public int? Rating { get; set; }
    }
}
