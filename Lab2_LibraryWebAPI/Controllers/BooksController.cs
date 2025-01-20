using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Lab2_LibraryWebAPI.Data;
using Lab2_LibraryWebAPI.DTOs;
using Lab2_LibraryWebAPI.Extensions;
using Lab2_LibraryWebAPI.Models;

namespace Lab2_LibraryWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly BooksDbContext _context;

        public BooksController(BooksDbContext context)
        {
            _context = context;
        }

        //default GET
        // GET: api/Books
        /*
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooks()
        {
            return await _context.Books.ToListAsync();
        }
        */

        // GET: api/Books
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DisplayBookDTO>>> GetBooks()
        {
            var displayBookDtos = await _context.Books
                .Include(b => b.Title)
                .Include(b => b.Series)
                .Include(b => b.Authors)
                .Include(b => b.Genre)
                .Include(b => b.Publisher)
                .Include(b => b.Edition)
                .Select(b => b.ToDisplayBookDTO())
                .ToListAsync();

            return displayBookDtos;
        }

        // GET: api/Books
        [HttpGet("GetBooksWithIds")]
        public async Task<ActionResult<IEnumerable<DisplayBookWithIdsDTO>>> GetBooksWithIds()
        {
            var displayBookWithIdsDtos = await _context.Books
                .Include(b => b.Title)
                .Include(b => b.Series)
                .Include(b => b.Authors)
                .Include(b => b.Genre)
                .Include(b => b.Publisher)
                .Include(b => b.Edition)
                .Select(b => b.ToDisplayBookWithIdsDTO())
                .ToListAsync();

            return displayBookWithIdsDtos;
        }

        //default GET
        // GET: api/Books/5
        /*
        [HttpGet("{id}")]
        public async Task<ActionResult<Book>> GetBook(int id)
        {
            var book = await _context.Books.FindAsync(id);

            if (book == null)
            {
                return NotFound();
            }

            return book;
        }
        */

        // GET: api/Books/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DisplayBookDTO>> GetBook(int id)
        {
            var book = await _context.Books.FindAsync(id);

            if (book == null)
                return NotFound();
            
            book = await _context.Books
                .Include(b => b.Title)
                .Include(b => b.Series)
                .Include(b => b.Authors)
                .Include(b => b.Genre)
                .Include(b => b.Publisher)
                .Include(b => b.Edition)
                .FirstOrDefaultAsync(b => b.Id == id);

            return book.ToDisplayBookDTO();
        }

        // GET: api/Books/5
        [HttpGet("GetBookWithId/{id}")]
        public async Task<ActionResult<DisplayBookWithIdsDTO>> GetBookWithIds(int id)
        {
            var book = await _context.Books.FindAsync(id);

            if (book == null)
                return NotFound();

            book = await _context.Books
                .Include(b => b.Title)
                .Include(b => b.Series)
                .Include(b => b.Authors)
                .Include(b => b.Genre)
                .Include(b => b.Publisher)
                .Include(b => b.Edition)
                .FirstOrDefaultAsync(b => b.Id == id);

            return book.ToDisplayBookWithIdsDTO();
        }

        // PUT: api/Books/5
        /*
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBook(int id, Book book)
        {
            if (id != book.Id)
            {
                return BadRequest();
            }

            _context.Entry(book).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookExists(id))
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

        //default POST
        // POST: api/Books
        /*
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Book>> PostBook(Book book)
        {
            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBook", new { id = book.Id }, book);
        }
        */

        // POST: api/Books
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Book>> PostBook(CreateBookDTO createBookDto)
        {
            //check if ISBN exists, and return error message if it does (as ISBN is a unique field)
            var existingISBN = await _context.Books.FirstOrDefaultAsync(b => b.ISBN == createBookDto.ISBN);
            if (existingISBN != null)
                return BadRequest("ISBN already exists in the database.");

            var book = await PopulateBookAsync(createBookDto.toBookWithoutAuthorDTO());
            book.Authors = await createBookDto.Authors.GetAuthorsAsync(_context);

            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBook", new { id = book.Id }, book);
        }

        // POST: api/Books
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("CreateBookWithAuthorId")]
        public async Task<ActionResult<Book>> PostBookWithAuthorId(CreateBookWithAuthorIdDTO createBookWithAuthorIdDto)
        {
            //check if ISBN exists, and return error message if it does (as ISBN is a unique field)
            var existingISBN = await _context.Books.FirstOrDefaultAsync(b => b.ISBN == createBookWithAuthorIdDto.ISBN);
            if (existingISBN != null)
                return BadRequest("ISBN already exists in the database.");

            foreach (var authorId in createBookWithAuthorIdDto.AuthorIds)
            {
                if (!await _context.Authors.AnyAsync(a => a.Id == authorId))
                    return BadRequest($"Author with Id {authorId} does not exist in the database.");
            }

            var bookWithoutAuthor = createBookWithAuthorIdDto.toBookWithoutAuthorDTO();
            var book = await PopulateBookAsync(bookWithoutAuthor);
            book.Authors = await createBookWithAuthorIdDto.AuthorIds.GetAuthorsAsync(_context);
            //book.Authors = await _context.Authors.GetAuthorsByIds(createBookWithAuthorIdDto.AuthorIds).ToListAsync();

            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBook", new { id = book.Id }, book);
        }

        // POST: api/Books
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("CreateBookWithIdsNewTitle")]
        public async Task<ActionResult<Book>> PostBookWithIdsNewTitle(CreateBookWithIdsNewTitleDTO createBookWithIdsNewTitleDto)
        {
            //check if ISBN exists, and return error message if it does (as ISBN is a unique field)
            var existingISBN = _context.Books.FirstOrDefault(b => b.ISBN == createBookWithIdsNewTitleDto.ISBN);
            if (existingISBN != null)
                return BadRequest("ISBN already exists in the database.");

            foreach (var authorId in createBookWithIdsNewTitleDto.AuthorIds)
            {
                if (!await _context.Authors.AnyAsync(a => a.Id == authorId))
                    return BadRequest($"Author with Id {authorId} does not exist in the database.");
            }

            var genre = await _context.Genres.FindAsync(createBookWithIdsNewTitleDto.GenreId);
            if (genre == null)
                return BadRequest($"Genre with Id {createBookWithIdsNewTitleDto.GenreId} does not exist in the database.");

            var publisher = await _context.Publishers.FindAsync(createBookWithIdsNewTitleDto.PublisherId);
            if (publisher == null)
                return BadRequest($"Publisher with Id {createBookWithIdsNewTitleDto.PublisherId} does not exist in the database.");

            var edition = await _context.Editions.FindAsync(createBookWithIdsNewTitleDto.EditionId);
            if (edition == null)
                return BadRequest($"Edition with Id {createBookWithIdsNewTitleDto.EditionId} does not exist in the database.");

            var book = new Book
            {
                Title = await createBookWithIdsNewTitleDto.Title.GetTitleAsync(_context),
                Series = createBookWithIdsNewTitleDto.Series == null ? null : await createBookWithIdsNewTitleDto.Series.GetSeriesAsync(_context),
                NumberInSeries = createBookWithIdsNewTitleDto.Series == null ? null : createBookWithIdsNewTitleDto.NumberInSeries,
                Authors = await createBookWithIdsNewTitleDto.AuthorIds.GetAuthorsAsync(_context),
                Genre = genre,
                ISBN = createBookWithIdsNewTitleDto.ISBN,
                PublishedYear = new DateOnly(createBookWithIdsNewTitleDto.PublishedYear, 1, 1),
                Publisher = publisher,
                Edition = edition,
                TotalQty = createBookWithIdsNewTitleDto.TotalQty,
                AvailableQty = createBookWithIdsNewTitleDto.AvailableQty
            };

            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBook", new { id = book.Id }, book);
        }

        // POST: api/Books
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("CreateBookWithIdsNewEdition")]
        public async Task<ActionResult<Book>> PostBookWithIdsNewEdition(CreateBookWithIdsNewEditionDTO createBookWithIdsNewEditionDto)
        {
            //check if ISBN exists, and return error message if it does (as ISBN is a unique field)
            var existingISBN = _context.Books.FirstOrDefault(b => b.ISBN == createBookWithIdsNewEditionDto.ISBN);
            if (existingISBN != null)
                return BadRequest("ISBN already exists in the database.");

            var title = await _context.Titles.FindAsync(createBookWithIdsNewEditionDto.TitleId);
            if (title == null)
                return BadRequest($"Title with Id {createBookWithIdsNewEditionDto.TitleId} does not exist in the database.");

            var series = await _context.Series.FindAsync(createBookWithIdsNewEditionDto.SeriesId);
            if (createBookWithIdsNewEditionDto.SeriesId != null)
            {
                if (series == null)
                    return BadRequest($"Series with Id {createBookWithIdsNewEditionDto.SeriesId} does not exist in the database.");
            }

            foreach (var authorId in createBookWithIdsNewEditionDto.AuthorIds)
            {
                if (!await _context.Authors.AnyAsync(a => a.Id == authorId))
                    return BadRequest($"Author with Id {authorId} does not exist in the database.");
            }

            var genre = await _context.Genres.FindAsync(createBookWithIdsNewEditionDto.GenreId);
            if (genre == null)
                return BadRequest($"Genre with Id {createBookWithIdsNewEditionDto.GenreId} does not exist in the database.");

            var book = new Book
            {
                Title = title,
                Series = createBookWithIdsNewEditionDto.SeriesId == null ? null : series,
                NumberInSeries = createBookWithIdsNewEditionDto.SeriesId == null ? null : createBookWithIdsNewEditionDto.NumberInSeries,
                Authors = await createBookWithIdsNewEditionDto.AuthorIds.GetAuthorsAsync(_context),
                Genre = genre,
                ISBN = createBookWithIdsNewEditionDto.ISBN,
                PublishedYear = new DateOnly(createBookWithIdsNewEditionDto.PublishedYear, 1, 1),
                Publisher = await createBookWithIdsNewEditionDto.Publisher.GetPublisherAsync(_context),
                Edition = await createBookWithIdsNewEditionDto.Edition.GetEditionAsync(_context),
                TotalQty = createBookWithIdsNewEditionDto.TotalQty,
                AvailableQty = createBookWithIdsNewEditionDto.AvailableQty
            };

            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBook", new { id = book.Id }, book);
        }

        //default DELETE
        // DELETE: api/Books/5
        /*
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        */

        // DELETE: api/Books/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
                return NotFound();

            //check if book has outstanding loans
            var hasLoans = await _context.Loans.AnyAsync(l => l.Book.Id == id && l.ReturnedDate == null);
            if (hasLoans)
                return BadRequest("Book has outstanding loans and cannot be deleted at this time.");

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BookExists(int id)
        {
            return _context.Books.Any(e => e.Id == id);
        }

        private async Task<Book> PopulateBookAsync(BookWithoutAuthorDTO bookWithoutAuthor)
        {
            //All of the Get[Item]Async (e.g. GetTitleAsync etc) extension methods check if [item] exists, uses existing [item] if it does, and creates a new [item] if it doesn't.
            return new Book
            {
                Title = await bookWithoutAuthor.Title.GetTitleAsync(_context),
                Series = bookWithoutAuthor.Series == null ? null : await bookWithoutAuthor.Series.GetSeriesAsync(_context),
                NumberInSeries = bookWithoutAuthor.Series == null ? null : bookWithoutAuthor.NumberInSeries,
                Genre = await bookWithoutAuthor.Genre.GetGenreAsync(_context),
                ISBN = bookWithoutAuthor.ISBN,
                PublishedYear = new DateOnly(bookWithoutAuthor.PublishedYear, 1, 1),
                Publisher = await bookWithoutAuthor.Publisher.GetPublisherAsync(_context),
                Edition = await bookWithoutAuthor.Edition.GetEditionAsync(_context),
                TotalQty = bookWithoutAuthor.TotalQty,
                AvailableQty = bookWithoutAuthor.AvailableQty
            };
        }
    }
}
