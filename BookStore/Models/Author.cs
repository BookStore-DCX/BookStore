using System;
using System.Collections.Generic;

namespace BookStore.Models;

public partial class Author
{
    public int AuthorId { get; set; }

    public string LastName { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string? Photo { get; set; }

    public virtual ICollection<Bookauthor> Bookauthors { get; set; } = new List<Bookauthor>();
}
