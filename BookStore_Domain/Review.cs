using System;
using System.Collections.Generic;

namespace BookStore_Domain;

public partial class Review
{
    public int CustomerId { get; set; }

    public string Isbn13 { get; set; } = null!;

    public int? Rating { get; set; }

    public string? Comment { get; set; }

    public DateTime? ReviewDatetime { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual Book Isbn13Navigation { get; set; } = null!;
}
