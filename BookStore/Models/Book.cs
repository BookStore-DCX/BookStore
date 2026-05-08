using System;
using System.Collections.Generic;

namespace BookStore.Models;

public partial class Book
{
    public string Isbn { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public int? Category { get; set; }

    public string? Edition { get; set; }

    public int PublisherId { get; set; }

    public virtual ICollection<Bookauthor> Bookauthors { get; set; } = new List<Bookauthor>();

    public virtual ICollection<Bookreview> Bookreviews { get; set; } = new List<Bookreview>();

    public virtual Category? CategoryNavigation { get; set; }

    public virtual ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();

    public virtual Publisher Publisher { get; set; } = null!;

    public virtual ICollection<Shoppingcart> Shoppingcarts { get; set; } = new List<Shoppingcart>();
}
