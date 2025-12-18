using System;
using System.Collections.Generic;

namespace BookStore_Domain;

public partial class Order
{
    public int OrderId { get; set; }

    public int CustomerId { get; set; }

    public int StoreId { get; set; }

    public DateTime OrderDatetime { get; set; }

    public string OrderStatus { get; set; } = null!;

    public virtual Customer Customer { get; set; } = null!;

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual Store Store { get; set; } = null!;
}
