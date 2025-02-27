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
    public class AuthorsController : ControllerBase
    {
        private readonly BooksDbContext _context;

        public AuthorsController(BooksDbContext context)
        {
            _context = context;
        }


        // GET: api/Authors
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Author>>> GetAuthors()
        {
            return await _context.Authors.ToListAsync();
        }


        // GET: api/Authors/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Author>> GetAuthor(int id)
        {
            var author = await _context.Authors.FindAsync(id);

            if (author == null)
            {
                return NotFound();
            }

            return author;
        }

        //default PUT
        // PUT: api/Authors/5
        /*
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAuthor(int id, Author author)
        {
            if (id != author.Id)
            {
                return BadRequest();
            }

            _context.Entry(author).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AuthorExists(id))
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

        // PUT: api/Authors/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("UpdateAuthor/{id}")]
        public async Task<IActionResult> PutAuthor(int id, AuthorNameDTO authorNameDto)
        {
            //check if the author to be edited exists
            if (!_context.Authors.TryGetAuthorById(id, out var author))
                return NotFound();

            //check if an author with the same name already exists
            if (_context.Authors.TryGetAuthorByName(authorNameDto.FirstName, authorNameDto.LastName, out var existingAuthor))
                return BadRequest($"An author with the same new name ({authorNameDto.FirstName} {authorNameDto.LastName}) already exists in the database with Id {existingAuthor.Id}.");

            author.FirstName = authorNameDto.FirstName;
            author.LastName = authorNameDto.LastName;

            _context.Entry(author).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AuthorExists(id))
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
        // POST: api/Authors
        /*
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Author>> PostAuthor(Author author)
        {
            _context.Authors.Add(author);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAuthor", new { id = author.Id }, author);
        }
        */

        // POST: api/Authors
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Author>> PostAuthor(AuthorNameDTO authorNameDto)
        {
            if (_context.Authors.TryGetAuthorByName(authorNameDto.FirstName, authorNameDto.LastName, out var existingAuthor))
                return BadRequest($"An author with the same name ({authorNameDto.FirstName} {authorNameDto.LastName}) already exists in the database with Id {existingAuthor.Id}.");

            var author = authorNameDto.ToAuthor();
            _context.Authors.Add(author);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAuthor", new { id = author.Id }, author);
        }

        //default DELETE
        // DELETE: api/Authors/5
        /*
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAuthor(int id)
        {
            var author = await _context.Authors.FindAsync(id);
            if (author == null)
            {
                return NotFound();
            }

            _context.Authors.Remove(author);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        */

        // DELETE: api/Authors/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAuthor(int id)
        {
            if (!_context.Authors.TryGetAuthorById(id, out var author))
                return NotFound();

            //check if author has any books
            var hasBooks = await _context.AuthorBook.AnyAsync(a => a.AuthorId == id);
            if (hasBooks)
                return BadRequest($"Author with Id {id} has books associated with it. Cannot delete author.");

            _context.Authors.Remove(author);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        private bool AuthorExists(int id)
        {
            return _context.Authors.Any(e => e.Id == id);
        }
    }
}
