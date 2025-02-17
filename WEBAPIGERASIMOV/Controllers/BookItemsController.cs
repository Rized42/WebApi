using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WEBAPIGERASIMOV.Models;

namespace WEBAPIGERASIMOV.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookItemsController : ControllerBase
    {
        private readonly BookContext _context;

        public BookItemsController(BookContext context)
        {
            _context = context;
        }

        // GET: api/BookItems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookItem>>> GetBooks()
        {
            return await _context.Books.ToListAsync();
        }

        // GET: api/BookItems/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BookItem>> GetBookItem(int id)
        {
            var bookItem = await _context.Books.FindAsync(id);

            if (bookItem == null)
            {
                return NotFound();
            }

            return bookItem;
        }

        // PUT: api/BookItems/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBookItem(int id, BookItem bookItem)
        {
            // Проверка, что ID в запросе совпадает с ID книги
            if (id != bookItem.Id)
            {
                return BadRequest("ID книги в запросе не совпадает с ID книги в теле запроса.");
            }

            // Валидация данных
            if (string.IsNullOrEmpty(bookItem.Title))
            {
                return BadRequest("Название книги не может быть пустым.");
            }

            if (string.IsNullOrEmpty(bookItem.Author))
            {
                return BadRequest("Автор книги не может быть пустым.");
            }

            // Проверка, существует ли книга с указанным ID
            var existingBook = await _context.Books.FindAsync(id);
            if (existingBook == null)
            {
                return NotFound("Книга с указанным ID не найдена.");
            }

            // Обновление данных книги
            existingBook.Title = bookItem.Title;
            existingBook.Author = bookItem.Author;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookItemExists(id))
                {
                    return NotFound("Книга с указанным ID не найдена.");
                }
                else
                {
                    throw;
                }
            }

            return NoContent(); // 204 No Content
        }

        // POST: api/BookItems
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<BookItem>> PostBookItem(BookItem bookItem)
        {
            _context.Books.Add(bookItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBookItem", new { id = bookItem.Id }, bookItem);
        }

        // DELETE: api/BookItems/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBookItem(int id)
        {
            // Поиск книги по ID
            var bookItem = await _context.Books.FindAsync(id);
            if (bookItem == null)
            {
                return NotFound("Книга с указанным ID не найдена.");
            }

            // Удаление книги
            _context.Books.Remove(bookItem);
            await _context.SaveChangesAsync();

            return NoContent(); // 204 No Content
        }

        private bool BookItemExists(int id)
        {
            return _context.Books.Any(e => e.Id == id);
        }
    }
}
