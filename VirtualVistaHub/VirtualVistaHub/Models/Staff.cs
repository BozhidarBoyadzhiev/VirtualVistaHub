using System;
using System.Collections.Generic;

namespace VirtualVistaHub.Models;

public partial class Staff
{
    public int UserId { get; set; }

    public string? UserLevel { get; set; }

    public virtual User User { get; set; } = null!;
}
