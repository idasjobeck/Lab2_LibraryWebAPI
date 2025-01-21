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
    public class TitlesController : ControllerBase
    {
        private readonly BooksDbContext _context;

        public TitlesController(BooksDbContext context)
        {
            _context = context;
        }

        // GET: api/Titles
        /*
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Title>>> GetTitles()
        {
            return await _context.Titles.ToListAsync();
        }
        */

        // GET: api/Titles/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Title>> GetTitle(int id)
        {
            var title = await _context.Titles.FindAsync(id);

            if (title == null)
            {
                return NotFound();
            }

            return title;
        }

        // PUT: api/Titles/5
        /*
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTitle(int id, Title title)
        {
            if (id != title.Id)
            {
                return BadRequest();
            }

            _context.Entry(title).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TitleExists(id))
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
        // POST: api/Titles
        /*
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Title>> PostTitle(Title title)
        {
            _context.Titles.Add(title);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTitle", new { id = title.Id }, title);
        }
        */

        // POST: api/Titles
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Title>> PostTitle(TitleNameDTO titleNameDto)
        {
            if (_context.Titles.TryGetTitleByName(titleNameDto.TitleName, out Title existingTitle))
                return BadRequest($"A title with the same name ({titleNameDto.TitleName}) already exists in the database with Id {existingTitle.Id}.");

            var title = titleNameDto.ToTitle();
            _context.Titles.Add(title);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTitle", new { id = title.Id }, title);
        }

        // DELETE: api/Titles/5
        /*
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTitle(int id)
        {
            var title = await _context.Titles.FindAsync(id);
            if (title == null)
            {
                return NotFound();
            }

            _context.Titles.Remove(title);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        */

        private bool TitleExists(int id)
        {
            return _context.Titles.Any(e => e.Id == id);
        }
    }
}
