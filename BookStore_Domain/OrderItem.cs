using System;
using System.Collections.Generic;

namespace BookStore_Domain;

public partial class OrderItem
{
    public int OrderId { get; set; }

    public string Isbn13 { get; set; } = null!;

    public int Quantity { get; set; }

    public decimal Price { get; set; }

    public virtual Book Isbn13Navigation { get; set; } = null!;

    public virtual Order Order { get; set; } = null!;
}
