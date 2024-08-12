using System;
using System.Collections.Generic;

namespace bookstoreapi.Models
{
    public class Genre
{
    public int GenreId { get; set; }
    public string GenreName { get; set; } = string.Empty; 
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public ICollection<Book> Books { get; set; } = new List<Book>(); 
}
}

