using System;
using System.Collections.Generic;

namespace BookStore_Domain;

public partial class TitlesPerAuthor
{
    public string Name { get; set; } = null!;

    public string Age { get; set; } = null!;

    public string Titles { get; set; } = null!;

    public string InventoryValue { get; set; } = null!;
}
