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
    public class GenresController : ControllerBase
    {
        private readonly BooksDbContext _context;

        public GenresController(BooksDbContext context)
        {
            _context = context;
        }

        // GET: api/Genres
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Genre>>> GetGenres()
        {
            return await _context.Genres.ToListAsync();
        }

        // GET: api/Genres/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Genre>> GetGenre(int id)
        {
            var genre = await _context.Genres.FindAsync(id);

            if (genre == null)
            {
                return NotFound();
            }

            return genre;
        }

        //default PUT
        // PUT: api/Genres/5
        /*
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGenre(int id, Genre genre)
        {
            if (id != genre.Id)
            {
                return BadRequest();
            }

            _context.Entry(genre).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GenreExists(id))
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

        // PUT: api/Genres/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("UpdateGenre/{id}")]
        public async Task<IActionResult> PutGenre(int id, GenreNameDTO genreNameDto)
        {
            //check if the genre to be edited exists
            if (!_context.Genres.TryGetGenreById(id, out var genre))
                return NotFound();

            //check if a genre with the same name already exists
            if (_context.Genres.TryGetGenreByName(genreNameDto.GenreName, out var existingGenre))
                return BadRequest($"A genre with the same new name ({genreNameDto.GenreName}) already exists in the database with Id {existingGenre.Id}.");

            genre.GenreName = genreNameDto.GenreName;

            _context.Entry(genre).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GenreExists(id))
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
        // POST: api/Genres
        /*
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Genre>> PostGenre(Genre genre)
        {
            _context.Genres.Add(genre);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetGenre", new { id = genre.Id }, genre);
        }
        */

        // POST: api/Genres
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Genre>> PostGenre(GenreNameDTO genreNameDto)
        {
            if(_context.Genres.TryGetGenreByName(genreNameDto.GenreName, out var existingGenre))
                return BadRequest($"A genre with the same name ({genreNameDto.GenreName}) already exists in the database with Id {existingGenre.Id}.");

            var genre = genreNameDto.ToGenre();
            _context.Genres.Add(genre);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetGenre", new { id = genre.Id }, genre);
        }

        // DELETE: api/Genres/5
        /*
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGenre(int id)
        {
            var genre = await _context.Genres.FindAsync(id);
            if (genre == null)
            {
                return NotFound();
            }

            _context.Genres.Remove(genre);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        */

        private bool GenreExists(int id)
        {
            return _context.Genres.Any(e => e.Id == id);
        }
    }
}
