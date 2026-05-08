using System;
using System.Collections.Generic;

namespace BookStore.Models;

public partial class Category
{
    public int CatId { get; set; }

    public string? CatDescription { get; set; }

    public virtual ICollection<Book> Books { get; set; } = new List<Book>();
}
