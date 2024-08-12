using System;
using System.Collections.Generic;

namespace bookstoreapi.Models
{
    public class Author
    {
        public int AuthorId { get; set; }
        public string AuthorName { get; set; } = string.Empty;
        public string Biography { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public ICollection<Book> Books { get; set; } = new List<Book>();
    }
}
