using System;
using System.Collections.Generic;

namespace BookStore_Domain;

public partial class Book
{
    public string Isbn13 { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string? Language { get; set; }

    public decimal? Price { get; set; }

    public DateOnly? PublicationDate { get; set; }

    public int? PageCount { get; set; }

    public int? GenreId { get; set; }

    public int? PublisherId { get; set; }

    public virtual ICollection<BookAuthor> BookAuthors { get; set; } = new List<BookAuthor>();

    public virtual Genre? Genre { get; set; }

    public virtual ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual Publisher? Publisher { get; set; }

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}
