using System;
using System.Collections.Generic;

namespace BookStore.Models;

public partial class Reviewer
{
    public int ReviewerId { get; set; }

    public string Name { get; set; } = null!;

    public string? EmployedBy { get; set; }

    public virtual ICollection<Bookreview> Bookreviews { get; set; } = new List<Bookreview>();
}
