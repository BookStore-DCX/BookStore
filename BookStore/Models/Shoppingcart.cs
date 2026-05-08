using System;
using System.Collections.Generic;

namespace BookStore.Models;

public partial class Shoppingcart
{
    public int UserId { get; set; }

    public string Isbn { get; set; } = null!;

    public virtual Book IsbnNavigation { get; set; } = null!;
}
