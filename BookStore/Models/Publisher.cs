using System;
using System.Collections.Generic;

namespace BookStore.Models;

public partial class Publisher
{
    public int PublisherId { get; set; }

    public string Name { get; set; } = null!;

    public string? City { get; set; }

    public string? StateCode { get; set; }

    public virtual ICollection<Book> Books { get; set; } = new List<Book>();

    public virtual State? StateCodeNavigation { get; set; }
}
