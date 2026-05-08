using System;
using System.Collections.Generic;

namespace BookStore.Models;

public partial class State
{
    public string StateCode { get; set; } = null!;

    public string? StateName { get; set; }

    public virtual ICollection<Publisher> Publishers { get; set; } = new List<Publisher>();
}
