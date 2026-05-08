using System;
using System.Collections.Generic;

namespace BookStore.Models;

public partial class Bookcondition
{
    public int Ranks { get; set; }

    public string? Description { get; set; }

    public string? FullDescription { get; set; }

    public decimal? Price { get; set; }

    public virtual ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();
}
