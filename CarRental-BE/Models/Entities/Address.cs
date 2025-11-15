using System;
using System.Collections.Generic;

namespace CarRental_BE.Models.Entities;

public partial class Address
{
    public Guid AddressId { get; set; }

    public string HouseNumberStreet { get; set; } = null!;

    public string Ward { get; set; } = null!;

    public string District { get; set; } = null!;

    public string CityProvince { get; set; } = null!;

    public decimal? Latitude { get; set; }

    public decimal? Longitude { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<BookingDriver> BookingDrivers { get; set; } = new List<BookingDriver>();

    public virtual ICollection<Booking> BookingDropOffAddresses { get; set; } = new List<Booking>();

    public virtual ICollection<Booking> BookingPickUpAddresses { get; set; } = new List<Booking>();

    public virtual ICollection<Car> Cars { get; set; } = new List<Car>();

    public virtual ICollection<UserProfile> UserProfiles { get; set; } = new List<UserProfile>();
}
