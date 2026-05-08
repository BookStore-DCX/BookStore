using System;
using System.Collections.Generic;

namespace BookStore.Models;

public partial class Inventory
{
    public int InventoryId { get; set; }

    public string Isbn { get; set; } = null!;

    public int Ranks { get; set; }

    public byte? Purchased { get; set; }

    public virtual Book IsbnNavigation { get; set; } = null!;

    public virtual Bookcondition RanksNavigation { get; set; } = null!;
}
