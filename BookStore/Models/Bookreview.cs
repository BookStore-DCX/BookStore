using System;
using System.Collections.Generic;

namespace BookStore.Models;

public partial class Bookreview
{
    public string Isbn { get; set; } = null!;

    public int ReviewerId { get; set; }

    public int? Rating { get; set; }

    public string? Comments { get; set; }

    public virtual Book IsbnNavigation { get; set; } = null!;

    public virtual Reviewer Reviewer { get; set; } = null!;
}
