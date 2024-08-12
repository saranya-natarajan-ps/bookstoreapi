using bookstoreapi.Models;
using System;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

public class BookSeeder
{
    private readonly BookstoreContext _context;
    private readonly Random _random = new Random();

    public BookSeeder(BookstoreContext context)
    {
        _context = context;
    }
public async Task SeedBooks()
{
    try
    {
        // Drop and recreate the entire database schema
        await _context.Database.EnsureDeletedAsync();  // Drops the database
        await _context.Database.EnsureCreatedAsync();  // Recreates the database with the current model

        // Seed predefined genres
        var predefinedGenres = new[] { "Fiction", "Non-Fiction", "Science", "Fantasy", "Biography", "Comedy" };
        foreach (var genreName in predefinedGenres)
        {
            try
            {
                // Ensure the genre doesn't already exist
                if (!_context.Genres.Any(g => g.GenreName == genreName))
                {
                    var genre = new Genre
                    {
                        GenreName = genreName,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };
                    _context.Genres.Add(genre);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while adding genre '{genreName}': {ex.Message}");
            }
        }
        await _context.SaveChangesAsync();

        // Fetch books from the Google Books API
        using (HttpClient client = new HttpClient())
        {
            int booksAdded = 0;
            int maxBooksToFetch = 100;
            int startIndex = 0;

            while (booksAdded < maxBooksToFetch)
            {
                string url = $"https://www.googleapis.com/books/v1/volumes?q=subject:fiction&langRestrict=hi&maxResults=40&startIndex={startIndex}&langRestrict=en,hi,ta,te,kn,ml,mr,bn";
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                string content = await response.Content.ReadAsStringAsync();
                JObject jsonResponse = JObject.Parse(content);

                var items = jsonResponse["items"];
                if (items == null || !items.Any())
                {
                    break; // No more books to fetch
                }

                foreach (var item in items)
                {
                    if (booksAdded >= maxBooksToFetch)
                    {
                        break; // Reached the limit of 100 books
                    }

                    var bookInfo = item["volumeInfo"];
                    if (bookInfo == null)
                    {
                        continue; // Skip if bookInfo is null
                    }


                    // Insert or retrieve Author
                    var authorName = bookInfo["authors"]?[0]?.ToString()?.Trim() ?? "Unknown Author";
                    var author = _context.Authors.SingleOrDefault(a => a.AuthorName == authorName);

                    // Check if author is valid before proceeding
                    if (author == null)
                    {
                        try
                        {
                            author = new Author
                            {
                                AuthorName = authorName,
                                Biography = "Biography not available",
                                CreatedAt = DateTime.Now,
                                UpdatedAt = DateTime.Now
                            };
                            _context.Authors.Add(author);
                            await _context.SaveChangesAsync();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"An error occurred while adding author '{authorName}': {ex.Message}");
                            // Skip this book if author creation failed
                            continue;
                        }
                    }

                    // Retrieve Genre
                    var genreName = bookInfo["categories"]?[0]?.ToString()?.Trim() ?? "Unknown Genre";
                    var genre = _context.Genres.SingleOrDefault(g => g.GenreName == genreName);

                    // Skip the book if genre doesn't exist
                    if (genre == null)
                    {
                        continue;
                    }

                    // Insert Book
                    var book = new Book
                    {
                        Title = bookInfo["title"]?.ToString() ?? "Untitled",
                        AuthorId = author.AuthorId,
                        GenreId = genre.GenreId,
                        Price = _random.Next(100, 600), // Random price between 100 and 600
                        PublicationDate = ParsePublicationDate(bookInfo["publishedDate"]?.ToString()),
                        Image = bookInfo["imageLinks"]?["thumbnail"]?.ToString() ?? "https://angelbookhouse.com/assets/front/img/product/edition_placeholder.png",
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };
                    try
                    {
                        _context.Books.Add(book);
                        booksAdded++;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"An error occurred while adding book '{book.Title}': {ex.Message}");
                        // Skip this book and continue
                    }
                }

                startIndex += 40; // Move to the next page of results
            }

            await _context.SaveChangesAsync();
        }
    }
    catch (Exception ex)
    {
        // Log or handle the exception
        Console.WriteLine($"An error occurred: {ex.Message}");
        // Optionally, handle specific scenarios where you want to return a random date
        var randomDate = GenerateRandomDate();
        Console.WriteLine($"Using random date: {randomDate}");
    }
}

private DateTime ParsePublicationDate(string publishedDate)
    {
        DateTime parsedDate;
        if (DateTime.TryParseExact(publishedDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate))
        {
            return parsedDate;
        }
        // Return a default date if parsing fails
        return new DateTime(1900, 1, 1);
    }

    private DateTime GenerateRandomDate()
    {
        var start = new DateTime(1900, 1, 1);
        var end = DateTime.Now;
        var range = (end - start).Days;
        var random = new Random();
        return start.AddDays(random.Next(range));
    }
}