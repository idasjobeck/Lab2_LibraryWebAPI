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
using Lab2_LibraryWebAPI.Extensions;
using System.Text.RegularExpressions;

namespace Lab2_LibraryWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly BooksDbContext _context;

        public UsersController(BooksDbContext context)
        {
            _context = context;
        }

        //default GET
        // GET: api/Users
        /*
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }
        */

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DisplayUserDTO>>> GetUsers()
        {
            var users = await _context.Users.ToListAsync();

            var usersToDisplay = new List<DisplayUserDTO>();
            foreach (var user in users)
                usersToDisplay.Add(user.ToDisplayUserDTO());

            return usersToDisplay;
        }

        // GET: api/Users
        [HttpGet("GetUsersWithIds")]
        public async Task<ActionResult<IEnumerable<DisplayUserWithIdDTO>>> GetUsersWithIds()
        {
            var users = await _context.Users.ToListAsync();

            var usersToDisplay = new List<DisplayUserWithIdDTO>();
            foreach (var user in users)
                usersToDisplay.Add(user.ToDisplayUserWithIdDTO());

            return usersToDisplay;
        }

        //default GET
        // GET: api/Users/5
        /*
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }
        */

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DisplayUserWithIdDTO>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
                return NotFound();

            return user.ToDisplayUserWithIdDTO();
        }

        // PUT: api/Users/5
        /*
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, User user)
        {
            if (id != user.Id)
            {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
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
        // POST: api/Users
        /*
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = user.Id }, user);
        }
        */

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(CreateUserDTO createUserDto)
        {
            //check if email already exists in db
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == createUserDto.Email);
            if (existingUser != null)
                return BadRequest(
                    $"A user with the same email ({createUserDto.Email}) already exists in the database with Id {existingUser.Id}.");

            //check email is in a valid format
            //regex pattern @"^[a-zA-Z0-9.!#$%&'*+-/=?^_`{|}~]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-]+(?:\.[a-zA-Z0-9-]+)*$"
            var emailValidationPattern = @"^[a-zA-Z0-9.!#$%&'*+-/=?^_`{|}~]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-]+(?:\.[a-zA-Z0-9-]+)*$";
            if (!Regex.IsMatch(createUserDto.Email, emailValidationPattern))
                return BadRequest("Email is not in a valid format.");

            var maxUser = 0;
            var baseCardNumber = 900000;
            //get max CardNumber from db, and assign new CardNumber to the new user
            if (UserExists(1))
                maxUser = _context.Users.Max(u => u.Id);

            var newUser = maxUser + 1;

            var user = new User
            {
                CardNumber = baseCardNumber + newUser,
                FirstName = createUserDto.FirstName,
                LastName = createUserDto.LastName,
                Email = createUserDto.Email
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = user.Id }, user);
        }

        //default DELETE
        // DELETE: api/Users/5
        /*
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        */

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound();

            //check if user has outstanding loans
            //var loans = await _context.Loans.Where(l => l.User.Id == id && l.ReturnedDate != null).ToListAsync();
            var hasLoans = await _context.Loans.Include(l => l.User).AnyAsync(l => l.User!.Id == id && l.ReturnedDate == null);
            if (hasLoans) //(loans.Count > 0)
                return BadRequest("User has outstanding loans and cannot be deleted at this time.");

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
