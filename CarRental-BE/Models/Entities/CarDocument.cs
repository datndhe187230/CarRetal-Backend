using System;
using System.Collections.Generic;

namespace CarRental_BE.Models.Entities;

public partial class CarDocument
{
    public Guid DocId { get; set; }

    public Guid CarId { get; set; }

    public string DocType { get; set; } = null!;

    public string Uri { get; set; } = null!;

    public bool? Verified { get; set; }

    public DateTime? VerifiedAt { get; set; }

    public DateOnly? ExpiresAt { get; set; }

    public virtual Car Car { get; set; } = null!;
}
