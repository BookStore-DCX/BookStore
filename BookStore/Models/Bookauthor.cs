using System;
using System.Collections.Generic;

namespace BookStore.Models;

public partial class Bookauthor
{
    public string Isbn { get; set; } = null!;

    public int AuthorId { get; set; }

    public string? PrimaryAuthor { get; set; }

    public virtual Author Author { get; set; } = null!;

    public virtual Book IsbnNavigation { get; set; } = null!;
}
