namespace Lab2_LibraryWebAPI.Models
{
    public class Loan
    {
        public int Id { get; set; }
        public required Book? Book { get; set; }
        public required User? User { get; set; }
        public required string LoanDate { get; set; }
        public string? ReturnedDate { get; set; }
        public int? Rating { get; set; }
    }
}
