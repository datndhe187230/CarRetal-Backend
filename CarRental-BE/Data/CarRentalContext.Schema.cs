using Microsoft.EntityFrameworkCore;

namespace CarRental_BE.Data
{
    public partial class CarRentalContext
    {
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("dbo");
        }
    }
}