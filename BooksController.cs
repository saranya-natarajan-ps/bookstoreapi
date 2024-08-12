using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using bookstoreapi.Models;  // Assuming your DbContext and models are in this namespace

[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly BookstoreContext _context;

    public BooksController(BookstoreContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Book>>> GetBooks()
    {
        return await _context.Books.Include(b => b.Author).Include(b => b.Genre).ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Book>> GetBook(int id)
    {
        var book = await _context.Books.Include(b => b.Author).Include(b => b.Genre).FirstOrDefaultAsync(b => b.BookId == id);

        if (book == null)
        {
            return NotFound();
        }

        return book;
    }

    [HttpPost]
    public async Task<ActionResult<Book>> PostBook(Book book)
    {
        if (book == null)
        {
            return BadRequest("Book cannot be null.");
        }

        // Set default image if not provided
        if (string.IsNullOrEmpty(book.Image))
        {
            book.Image = "https://angelbookhouse.com/assets/front/img/product/edition_placeholder.png";
        }

        // Ensure the AuthorId and GenreId are correctly associated
        if (!_context.Authors.Any(a => a.AuthorId == book.AuthorId))
        {
            return BadRequest("Invalid AuthorId.");
        }

        if (!_context.Genres.Any(g => g.GenreId == book.GenreId))
        {
            return BadRequest("Invalid GenreId.");
        }

        // Attach the existing Author and Genre to avoid EF trying to insert new records
        _context.Entry(book).Reference(b => b.Author).IsModified = false;
        _context.Entry(book).Reference(b => b.Genre).IsModified = false;

        _context.Books.Add(book);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetBook), new { id = book.BookId }, book);
    }


    [HttpPut("{id}")]
    public async Task<IActionResult> PutBook(int id, Book book)
    {
        if (id != book.BookId)
        {
            return BadRequest("Book ID mismatch.");
        }

        _context.Entry(book).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Books.Any(e => e.BookId == id))
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
}
