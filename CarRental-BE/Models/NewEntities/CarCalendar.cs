using System;
using System.Collections.Generic;

namespace CarRental_BE.Models.NewEntities;

public partial class CarCalendar
{
    public Guid CalendarId { get; set; }

    public Guid CarId { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public string? Reason { get; set; }

    public virtual Car Car { get; set; } = null!;
}
