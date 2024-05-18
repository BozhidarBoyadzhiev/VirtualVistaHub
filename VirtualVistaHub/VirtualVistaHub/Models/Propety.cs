using System;
using System.Collections.Generic;

namespace VirtualVistaHub.Models;

public partial class Propety
{
    public int PropertyId { get; set; }

    public string? TypeOfProperty { get; set; }

    public string? District { get; set; }

    public int? Price { get; set; }

    public int? Area { get; set; }

    public string? TypeOfContrusction { get; set; }

    public string? PhoneNumber { get; set; }

    public string? AdditionalInformation { get; set; }

    public string? ApprovalStatus { get; set; }

    public bool? Deleted { get; set; }

    public bool? Sold { get; set; }

    public int? UserId { get; set; }

    public virtual User? User { get; set; }
}
