using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using WEBAPIGERASIMOV.Models;

namespace WEBAPIGERASIMOV.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserItemsController : ControllerBase
    {
        private readonly UserContext _context;

        public UserItemsController(UserContext context)
        {
            _context = context;
        }

        // GET: api/UserItems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserItem>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        // GET: api/UserItems/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserItem>> GetUserItem(int id)
        {
            var userItem = await _context.Users.FindAsync(id);

            if (userItem == null)
            {
                return NotFound();
            }

            return userItem;
        }

        // PUT: api/UserItems/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUserItem(int id, UserItem userItem)
        {
            if (id != userItem.Id)
            {
                return BadRequest();
            }

            _context.Entry(userItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserItemExists(id))
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

        // POST: api/UserItems
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<UserItem>> PostUserItem(UserItem userItem)
        {
            _context.Users.Add(userItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUserItem", new { id = userItem.Id }, userItem);
        }

        // DELETE: api/UserItems/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserItem(int id)
        {
            var userItem = await _context.Users.FindAsync(id);
            if (userItem == null)
            {
                return NotFound();
            }

            _context.Users.Remove(userItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/UserItems/register
        [HttpPost("register")]
        public async Task<ActionResult<UserItem>> Register([FromBody] UserItem userItem)
        {
            if (userItem == null || string.IsNullOrEmpty(userItem.Email) || string.IsNullOrEmpty(userItem.password))
            {
                return BadRequest("Invalid user data.");
            }

            // Проверка, существует ли пользователь с таким именем
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == userItem.Email);
            if (existingUser != null)
            {
                return Conflict("User with this email already exists.");
            }

            // Сохранение пользователя в базу данных
            _context.Users.Add(userItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUserItem", new { id = userItem.Id }, userItem);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserItem userItem)
        {
            if (userItem == null || string.IsNullOrEmpty(userItem.Email) || string.IsNullOrEmpty(userItem.password))
            {
                return BadRequest("Invalid user data.");
            }

            // Поиск пользователя в базе данных
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userItem.Email && u.password == userItem.password);
            if (user == null)
            {
                return Unauthorized("Invalid username or password.");
            }

            return Ok();
        }

        private bool UserItemExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
