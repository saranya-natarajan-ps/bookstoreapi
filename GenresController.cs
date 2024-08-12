using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using bookstoreapi.Models;  // Assuming your DbContext and models are in this namespace

[ApiController]
[Route("api/[controller]")]
public class GenresController : ControllerBase
{
    private readonly BookstoreContext _context;

    public GenresController(BookstoreContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Genre>>> GetGenres()
    {
        return await _context.Genres.ToListAsync();
    }

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

    [HttpPost]
    public async Task<ActionResult<Genre>> PostGenre(Genre genre)
    {
        if (genre == null)
        {
            return BadRequest("Genre cannot be null.");
        }

        _context.Genres.Add(genre);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetGenre), new { id = genre.GenreId }, genre);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutGenre(int id, Genre genre)
    {
        if (id != genre.GenreId)
        {
            return BadRequest("Genre ID mismatch.");
        }

        var existingGenre = await _context.Genres.FindAsync(id);
        if (existingGenre == null)
        {
            return NotFound();
        }

        // Update the genre
        existingGenre.GenreName = genre.GenreName;

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

    private bool GenreExists(int id)
    {
        return _context.Genres.Any(e => e.GenreId == id);
    }
}
