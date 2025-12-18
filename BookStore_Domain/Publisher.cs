using System;
using System.Collections.Generic;

namespace BookStore_Domain;

public partial class Publisher
{
    public int PublisherId { get; set; }

    public string PublisherName { get; set; } = null!;

    public string? Address { get; set; }

    public string? City { get; set; }

    public string? Country { get; set; }

    public virtual ICollection<Book> Books { get; set; } = new List<Book>();
}
