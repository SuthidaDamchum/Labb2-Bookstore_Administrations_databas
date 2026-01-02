using System;
using System.Collections.Generic;

namespace BookStore_Domain;

public partial class BookAuthor
{
    public string BookIsbn13 { get; set; } = null!; // FK to Book
    public int AuthorId { get; set; }                // FK to Author    
    public string? Role { get; set; }
    public virtual Author Author { get; set; } = null!;
    public virtual Book Book { get; set; } = null!;    // single navigation property

}
