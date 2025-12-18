using System;
using System.Collections.Generic;

namespace BookStore_Domain;

public partial class BookAuthor
{
    public string BookIsbn13 { get; set; } = null!;

    public int AuthorId { get; set; }

    public string? Role { get; set; }

    public virtual Author Author { get; set; } = null!;

    public virtual Book BookIsbn13Navigation { get; set; } = null!;
}
