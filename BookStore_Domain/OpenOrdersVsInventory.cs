using System;
using System.Collections.Generic;

namespace BookStore_Domain;

public partial class OpenOrdersVsInventory
{
    public string StoreName { get; set; } = null!;

    public string Isbn13 { get; set; } = null!;

    public string Title { get; set; } = null!;

    public int? OrderedNotDelivered { get; set; }

    public int InventoryQuantity { get; set; }

    public int? RemainingAfterOrders { get; set; }
}
