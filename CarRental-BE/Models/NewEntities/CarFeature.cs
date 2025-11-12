using System;
using System.Collections.Generic;

namespace CarRental_BE.Models.NewEntities;

public partial class CarFeature
{
    public int FeatureId { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Car> Cars { get; set; } = new List<Car>();
}
