using System;
using System.Collections.Generic;

namespace CarRental_BE.Models.NewEntities;

public partial class Account
{
    public Guid AccountId { get; set; }

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public int RoleId { get; set; }

    public bool IsActive { get; set; }

    public bool IsEmailVerified { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<BookingDriver> BookingDrivers { get; set; } = new List<BookingDriver>();

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<Car> Cars { get; set; } = new List<Car>();

    public virtual ICollection<Review> ReviewFromAccounts { get; set; } = new List<Review>();

    public virtual ICollection<Review> ReviewToAccounts { get; set; } = new List<Review>();

    public virtual Role Role { get; set; } = null!;

    public virtual UserProfile? UserProfile { get; set; }

    public virtual Wallet? Wallet { get; set; }
}
