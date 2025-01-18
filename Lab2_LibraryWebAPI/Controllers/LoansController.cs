using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Lab2_LibraryWebAPI.Data;
using Lab2_LibraryWebAPI.Models;
using Lab2_LibraryWebAPI.DTOs;

namespace Lab2_LibraryWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoansController : ControllerBase
    {
        private readonly BooksDbContext _context;

        public LoansController(BooksDbContext context)
        {
            _context = context;
        }

        // GET: api/Loans
        /*
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Loan>>> GetLoans()
        {
            return await _context.Loans.ToListAsync();
        }
        */

        // GET: api/Loans/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Loan>> GetLoan(int id)
        {
            var loan = await _context.Loans.FindAsync(id);

            if (loan == null)
            {
                return NotFound();
            }

            return loan;
        }

        // PUT: api/Loans/5
        /*
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLoan(int id, Loan loan)
        {
            if (id != loan.Id)
            {
                return BadRequest();
            }

            _context.Entry(loan).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LoanExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }
        */

        // PUT: api/Loans/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("ReturnLoan/{id}")]
        public async Task<IActionResult> ReturnLoan(int id, ReturnLoanDTO returnLoanDto)
        {
            //check if loan exists
            //var loan = await _context.Loans.FindAsync(id);
            var loan = await _context.Loans.Include(l => l.Book).FirstOrDefaultAsync(l => l.Id == id);
            if (loan == null)
                return BadRequest("Loan not found.");
            //check if book is returned
            if (loan.ReturnedDate != null)
                return BadRequest("Book is already returned.");
            //check if rating is between 1 and 5
            if (returnLoanDto.BookRating < 1 || returnLoanDto.BookRating > 5)
                return BadRequest("Rating must be between 1 and 5 (or null if no rating provided).");

            loan.ReturnedDate = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
            loan.Rating = returnLoanDto.BookRating;
            var book = await _context.Books.FindAsync(loan.Book.Id);
            book!.AvailableQty++;
            //if available qty is greater than total qty, set available qty to total qty
            if (book.AvailableQty > book.TotalQty)
                book.AvailableQty = book.TotalQty;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LoanExists(id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        //default POST
        // POST: api/Loans
        /*
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Loan>> PostLoan(Loan loan)
        {
            _context.Loans.Add(loan);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetLoan", new { id = loan.Id }, loan);
        }
        */

        // POST: api/Loans
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Loan>> PostLoan(CreateLoanDTO createLoanDto)
        {
            //check if book exists
            var book = await _context.Books.Include(b => b.Title).FirstOrDefaultAsync(b => b.Title.TitleName == createLoanDto.Title);
            if (book == null)
                return BadRequest("Book not found.");
            //check if book is available
            if (book.AvailableQty == 0)
                return BadRequest("There are no copies available of this book.");
            //check if user exists
            var user = await _context.Users.FirstOrDefaultAsync(u => u.CardNumber == createLoanDto.CardNumber);
            if (user == null)
                return BadRequest("User not found.");

            var loan = new Loan
            {
                Book = book,
                User = user,
                LoanDate = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"),
                ReturnedDate = null,
                Rating = null
            };
            book.AvailableQty--;

            _context.Loans.Add(loan);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetLoan", new { id = loan.Id }, loan);
        }

        // POST: api/Loans
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("CreateLoanWithIds")]
        public async Task<ActionResult<Loan>> CreateLoanWithIds(CreateLoanWithIdsDTO createLoanWithIdsDto)
        {
            //check if book exists
            var book = await _context.Books.FindAsync(createLoanWithIdsDto.BookId);
            if (book == null)
                return BadRequest("Book not found.");
            //check if book is available
            if (book.AvailableQty == 0)
                return BadRequest("There are no copies available of this book.");
            //check if user exists
            var user = await _context.Users.FindAsync(createLoanWithIdsDto.UserId);
            if (user == null)
                return BadRequest("User not found.");

            var loan = new Loan
            {
                Book = book,
                User = user,
                LoanDate = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"),
                ReturnedDate = null,
                Rating = null
            };
            book.AvailableQty--;

            _context.Loans.Add(loan);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetLoan", new { id = loan.Id }, loan);
        }

        // DELETE: api/Loans/5
        /*
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLoan(int id)
        {
            var loan = await _context.Loans.FindAsync(id);
            if (loan == null)
            {
                return NotFound();
            }

            _context.Loans.Remove(loan);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        */

        private bool LoanExists(int id)
        {
            return _context.Loans.Any(e => e.Id == id);
        }
    }
}
