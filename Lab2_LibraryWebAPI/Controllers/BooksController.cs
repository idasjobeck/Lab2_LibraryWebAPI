﻿using System;
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

            return book!.ToDisplayBookDTO();
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

            return book!.ToDisplayBookWithIdsDTO();
        }

        //default PUT
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

        // PUT: api/Books/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("UpdateQuantity/{id}")]
        public async Task<IActionResult> UpdateBookQuantities(int id, BookQtyDTO bookQtyDto)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
                return BadRequest($"Book with Id {id} does not exist.");

            //check if available qty is higher than total qty
            if (bookQtyDto.AvailableQty > bookQtyDto.TotalQty)
                return BadRequest("Available quantity cannot be higher than total quantity.");

            var currentOnLoanQty = await _context.Loans.Where(l => l.Book!.Id == id && l.ReturnedDate == null).CountAsync();
            //new total quantity cannot be lower than the number of books currently on loan
            if (bookQtyDto.TotalQty < currentOnLoanQty)
                return BadRequest("New total quantity cannot be lower than the number of books currently on loan.");
            //new available quantity cannot be lower than the number of books currently on loan
            if (bookQtyDto.AvailableQty < currentOnLoanQty)
                return BadRequest("New available quantity cannot be lower than the number of books currently on loan.");

            book.TotalQty = bookQtyDto.TotalQty;
            book.AvailableQty = bookQtyDto.AvailableQty - currentOnLoanQty;

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

            //check if available qty is higher than total qty
            if (createBookDto.AvailableQty > createBookDto.TotalQty)
                return BadRequest("Available quantity cannot be higher than total quantity.");

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

            //check if available qty is higher than total qty
            if (createBookWithAuthorIdDto.AvailableQty > createBookWithAuthorIdDto.TotalQty)
                return BadRequest("Available quantity cannot be higher than total quantity.");

            if (!_context.Authors.AuthorsExists(createBookWithAuthorIdDto.AuthorIds))
                return BadRequest("One or more authors do not exist in the database.");

            var bookWithoutAuthor = createBookWithAuthorIdDto.toBookWithoutAuthorDTO();
            var book = await PopulateBookAsync(bookWithoutAuthor);
            book.Authors = await _context.Authors.GetAuthorsByIds(createBookWithAuthorIdDto.AuthorIds).ToListAsync();

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

            //check if available qty is higher than total qty
            if (createBookWithIdsNewTitleDto.AvailableQty > createBookWithIdsNewTitleDto.TotalQty)
                return BadRequest("Available quantity cannot be higher than total quantity.");

            if (!_context.Authors.AuthorsExists(createBookWithIdsNewTitleDto.AuthorIds))
                return BadRequest("One or more authors do not exist in the database.");

            if(!_context.Genres.TryGetGenreById(createBookWithIdsNewTitleDto.GenreId, out var genre))
                return BadRequest($"Genre with Id {createBookWithIdsNewTitleDto.GenreId} does not exist in the database.");

            if (!_context.Publishers.TryGetPublisherById(createBookWithIdsNewTitleDto.PublisherId, out var publisher))
                return BadRequest($"Publisher with Id {createBookWithIdsNewTitleDto.PublisherId} does not exist in the database.");

            if (!_context.Editions.TryGetEditionById(createBookWithIdsNewTitleDto.EditionId, out var edition))
                return BadRequest($"Edition with Id {createBookWithIdsNewTitleDto.EditionId} does not exist in the database.");

            var book = new Book
            {
                Title = await _context.Titles.GetTitleAsync(createBookWithIdsNewTitleDto.Title),
                Series = createBookWithIdsNewTitleDto.Series == null ? null : await _context.Series.GetSeriesAsync(createBookWithIdsNewTitleDto.Series),
                NumberInSeries = createBookWithIdsNewTitleDto.Series == null ? null : createBookWithIdsNewTitleDto.NumberInSeries,
                Authors = await _context.Authors.GetAuthorsByIds(createBookWithIdsNewTitleDto.AuthorIds).ToListAsync(),
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

            //check if available qty is higher than total qty
            if (createBookWithIdsNewEditionDto.AvailableQty > createBookWithIdsNewEditionDto.TotalQty)
                return BadRequest("Available quantity cannot be higher than total quantity.");

            if (!_context.Titles.TryGetTitleById(createBookWithIdsNewEditionDto.TitleId, out var title))
                return BadRequest($"Title with Id {createBookWithIdsNewEditionDto.TitleId} does not exist in the database.");

            Series? series = null;
            if (createBookWithIdsNewEditionDto.SeriesId != null)
            {
                if (!_context.Series.TryGetSeriesById(createBookWithIdsNewEditionDto.SeriesId, out series))
                    return BadRequest($"Series with Id {createBookWithIdsNewEditionDto.SeriesId} does not exist in the database.");
            }

            if (!_context.Authors.AuthorsExists(createBookWithIdsNewEditionDto.AuthorIds))
                return BadRequest("One or more authors do not exist in the database.");

            if (!_context.Genres.TryGetGenreById(createBookWithIdsNewEditionDto.GenreId, out var genre))
                return BadRequest($"Genre with Id {createBookWithIdsNewEditionDto.GenreId} does not exist in the database.");

            var book = new Book
            {
                Title = title,
                Series = createBookWithIdsNewEditionDto.SeriesId == null ? null : series,
                NumberInSeries = createBookWithIdsNewEditionDto.SeriesId == null ? null : createBookWithIdsNewEditionDto.NumberInSeries,
                Authors = await _context.Authors.GetAuthorsByIds(createBookWithIdsNewEditionDto.AuthorIds).ToListAsync(),
                Genre = genre,
                ISBN = createBookWithIdsNewEditionDto.ISBN,
                PublishedYear = new DateOnly(createBookWithIdsNewEditionDto.PublishedYear, 1, 1),
                Publisher = await _context.Publishers.GetPublisherAsync(createBookWithIdsNewEditionDto.Publisher),
                Edition = await _context.Editions.GetEditionAsync(createBookWithIdsNewEditionDto.Edition),
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
            var hasLoans = await _context.Loans.AnyAsync(l => l.Book!.Id == id && l.ReturnedDate == null);
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
                Title = await _context.Titles.GetTitleAsync(bookWithoutAuthor.Title),
                Series = bookWithoutAuthor.Series == null ? null : await _context.Series.GetSeriesAsync(bookWithoutAuthor.Series),
                NumberInSeries = bookWithoutAuthor.Series == null ? null : bookWithoutAuthor.NumberInSeries,
                Genre = await _context.Genres.GetGenreAsync(bookWithoutAuthor.Genre),
                ISBN = bookWithoutAuthor.ISBN,
                PublishedYear = new DateOnly(bookWithoutAuthor.PublishedYear, 1, 1),
                Publisher = await _context.Publishers.GetPublisherAsync(bookWithoutAuthor.Publisher),
                Edition = await _context.Editions.GetEditionAsync(bookWithoutAuthor.Edition),
                TotalQty = bookWithoutAuthor.TotalQty,
                AvailableQty = bookWithoutAuthor.AvailableQty
            };
        }
    }
}
