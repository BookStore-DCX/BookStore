using System;
using System.Collections.Generic;

namespace BookStore.Models;

public partial class User
{
    public int UserId { get; set; }

    public string LastName { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string? PhoneNumber { get; set; }

    public string UserName { get; set; } = null!;

    public string Password { get; set; } = null!;

    public int? RoleNumber { get; set; }

    public virtual ICollection<Purchaselog> Purchaselogs { get; set; } = new List<Purchaselog>();

    public virtual Permrole? RoleNumberNavigation { get; set; }
}
