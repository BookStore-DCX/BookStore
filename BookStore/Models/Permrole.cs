using System;
using System.Collections.Generic;

namespace BookStore.Models;

public partial class Permrole
{
    public int RoleNumber { get; set; }

    public string? PermRole1 { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
