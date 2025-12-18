using System;
using System.Collections.Generic;

namespace BookStore_Domain;

public partial class Store
{
    public int StoreId { get; set; }

    public string StoreName { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string City { get; set; } = null!;

    public string PostalCode { get; set; } = null!;

    public string Country { get; set; } = null!;

    public virtual ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
