using System;
using System.ComponentModel.DataAnnotations;

namespace bookstoreapi.Models
{
   public class Book
{
    public int BookId { get; set; }

    [Required]
    public string Title { get; set; } = string.Empty;

    [Required]
    public int AuthorId { get; set; }

    public Author? Author { get; set; }

    [Required]
    public int GenreId { get; set; }

    public Genre? Genre { get; set; }

    public decimal Price { get; set; }

    public DateTime? PublicationDate { get; set; }

    public string Image { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}

}
