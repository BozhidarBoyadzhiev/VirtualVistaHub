using System;
using System.Collections.Generic;

namespace VirtualVistaHub.Models;

public partial class User
{
    public int UserId { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Email { get; set; }

    public string? Password { get; set; }

    public bool? Deleted { get; set; }

    public virtual ICollection<Propety> Propeties { get; set; } = new List<Propety>();

    public virtual Staff? Staff { get; set; }
}
