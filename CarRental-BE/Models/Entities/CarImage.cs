using System;
using System.Collections.Generic;

namespace CarRental_BE.Models.Entities;
public partial class CarImage
{
    public Guid ImageId { get; set; }

    public Guid CarId { get; set; }

    public string ImageType { get; set; } = null!;

    public string Uri { get; set; } = null!;

    public bool? IsPrimary { get; set; }

    public DateTime UploadedAt { get; set; }

    public virtual Car Car { get; set; } = null!;
}
