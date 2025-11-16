using CarRental_BE.Models.NewEntities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace CarRental_BE.Data;

public partial class CarRentalContext : DbContext
{
    public CarRentalContext()
    {
    }

    public CarRentalContext(DbContextOptions<CarRentalContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<Address> Addresses { get; set; }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<BookingDriver> BookingDrivers { get; set; }

    public virtual DbSet<BookingStatusHistory> BookingStatusHistories { get; set; }

    public virtual DbSet<Car> Cars { get; set; }

    public virtual DbSet<CarCalendar> CarCalendars { get; set; }

    public virtual DbSet<CarDocument> CarDocuments { get; set; }

    public virtual DbSet<CarFeature> CarFeatures { get; set; }

    public virtual DbSet<CarImage> CarImages { get; set; }

    public virtual DbSet<CarPricingPlan> CarPricingPlans { get; set; }

    public virtual DbSet<Promotion> Promotions { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Transaction> Transactions { get; set; }

    public virtual DbSet<UserProfile> UserProfiles { get; set; }

    public virtual DbSet<Wallet> Wallets { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.AccountId).HasName("PK__account__46A222CD59E78EB1");

            entity.ToTable("account");

            entity.HasIndex(e => e.Email, "UQ__account__AB6E61644BCE2579").IsUnique();

            entity.Property(e => e.AccountId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("account_id");
            entity.Property(e => e.CreatedAt)
                .HasPrecision(6)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.IsEmailVerified).HasColumnName("is_email_verified");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .HasColumnName("password_hash");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.UpdatedAt)
                .HasPrecision(6)
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Role).WithMany(p => p.Accounts)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__account__role_id__74AE54BC");
        });

        modelBuilder.Entity<Address>(entity =>
        {
            entity.HasKey(e => e.AddressId).HasName("PK__address__CAA247C8340BE9EC");

            entity.ToTable("address");

            entity.Property(e => e.AddressId)
                .ValueGeneratedNever()
                .HasColumnName("address_id");
            entity.Property(e => e.CityProvince)
                .HasMaxLength(100)
                .HasColumnName("city_province");
            entity.Property(e => e.CreatedAt)
                .HasPrecision(6)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("created_at");
            entity.Property(e => e.District)
                .HasMaxLength(100)
                .HasColumnName("district");
            entity.Property(e => e.HouseNumberStreet)
                .HasMaxLength(255)
                .HasColumnName("house_number_street");
            entity.Property(e => e.Latitude)
                .HasColumnType("decimal(9, 6)")
                .HasColumnName("latitude");
            entity.Property(e => e.Longitude)
                .HasColumnType("decimal(9, 6)")
                .HasColumnName("longitude");
            entity.Property(e => e.UpdatedAt)
                .HasPrecision(6)
                .HasColumnName("updated_at");
            entity.Property(e => e.Ward)
                .HasMaxLength(100)
                .HasColumnName("ward");
        });

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.BookingNumber).HasName("PK__booking__3A30D2BD3749B6C4");

            entity.ToTable("booking");

            entity.Property(e => e.BookingNumber)
                .HasMaxLength(20)
                .HasColumnName("booking_number");
            entity.Property(e => e.ActualReturnTime)
                .HasPrecision(6)
                .HasColumnName("actual_return_time");
            entity.Property(e => e.BasePriceSnapshotCents)
                .HasColumnType("decimal(19, 2)")
                .HasColumnName("base_price_snapshot_cents");
            entity.Property(e => e.CarId).HasColumnName("car_id");
            entity.Property(e => e.CreatedAt)
                .HasPrecision(6)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("created_at");
            entity.Property(e => e.DepositSnapshotCents)
                .HasColumnType("decimal(19, 2)")
                .HasColumnName("deposit_snapshot_cents");
            entity.Property(e => e.DropOffAddressId).HasColumnName("drop_off_address_id");
            entity.Property(e => e.DropOffTime)
                .HasPrecision(6)
                .HasColumnName("drop_off_time");
            entity.Property(e => e.ExtraChargesCents)
                .HasColumnType("decimal(19, 2)")
                .HasColumnName("extra_charges_cents");
            entity.Property(e => e.KmDriven)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("km_driven");
            entity.Property(e => e.PaymentMethod)
                .HasMaxLength(20)
                .HasColumnName("payment_method");
            entity.Property(e => e.PickUpAddressId).HasColumnName("pick_up_address_id");
            entity.Property(e => e.PickUpTime)
                .HasPrecision(6)
                .HasColumnName("pick_up_time");
            entity.Property(e => e.PricingPlanId).HasColumnName("pricing_plan_id");
            entity.Property(e => e.RenterAccountId).HasColumnName("renter_account_id");
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .HasColumnName("status");
            entity.Property(e => e.UpdatedAt)
                .HasPrecision(6)
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Car).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.CarId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__booking__car_id__75A278F5");

            entity.HasOne(d => d.DropOffAddress).WithMany(p => p.BookingDropOffAddresses)
                .HasForeignKey(d => d.DropOffAddressId)
                .HasConstraintName("FK__booking__drop_of__76969D2E");

            entity.HasOne(d => d.PickUpAddress).WithMany(p => p.BookingPickUpAddresses)
                .HasForeignKey(d => d.PickUpAddressId)
                .HasConstraintName("FK__booking__pick_up__778AC167");

            entity.HasOne(d => d.PricingPlan).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.PricingPlanId)
                .HasConstraintName("FK__booking__pricing__787EE5A0");

            entity.HasOne(d => d.RenterAccount).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.RenterAccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__booking__renter___797309D9");
        });

        modelBuilder.Entity<BookingDriver>(entity =>
        {
            entity.HasKey(e => e.BookingDriverId).HasName("PK__booking___4046DE41CCA78292");

            entity.ToTable("booking_driver");

            entity.Property(e => e.BookingDriverId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("booking_driver_id");
            entity.Property(e => e.AccountId).HasColumnName("account_id");
            entity.Property(e => e.AddressId).HasColumnName("address_id");
            entity.Property(e => e.BookingNumber)
                .HasMaxLength(20)
                .HasColumnName("booking_number");
            entity.Property(e => e.Dob).HasColumnName("dob");
            entity.Property(e => e.DrivingLicenseUri)
                .HasMaxLength(500)
                .HasColumnName("driving_license_uri");
            entity.Property(e => e.FullName)
                .HasMaxLength(255)
                .HasColumnName("full_name");
            entity.Property(e => e.NationalId)
                .HasMaxLength(20)
                .HasColumnName("national_id");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(20)
                .HasColumnName("phone_number");

            entity.HasOne(d => d.Account).WithMany(p => p.BookingDrivers)
                .HasForeignKey(d => d.AccountId)
                .HasConstraintName("FK__booking_d__accou__7A672E12");

            entity.HasOne(d => d.Address).WithMany(p => p.BookingDrivers)
                .HasForeignKey(d => d.AddressId)
                .HasConstraintName("FK__booking_d__addre__7B5B524B");

            entity.HasOne(d => d.BookingNumberNavigation).WithMany(p => p.BookingDrivers)
                .HasForeignKey(d => d.BookingNumber)
                .HasConstraintName("FK__booking_d__booki__7C4F7684");
        });

        modelBuilder.Entity<BookingStatusHistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__booking___3213E83FAC6CA306");

            entity.ToTable("booking_status_history");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BookingNumber)
                .HasMaxLength(20)
                .HasColumnName("booking_number");
            entity.Property(e => e.ChangedAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("changed_at");
            entity.Property(e => e.NewStatus)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("new_status");
            entity.Property(e => e.Note)
                .HasMaxLength(255)
                .HasColumnName("note");
            entity.Property(e => e.OldStatus)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("old_status");
            entity.Property(e => e.PictureUrl)
                .HasMaxLength(500)
                .HasColumnName("picture_url");

            entity.HasOne(d => d.BookingNumberNavigation).WithMany(p => p.BookingStatusHistories)
                .HasForeignKey(d => d.BookingNumber)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_booking_status_history_booking");
        });

        modelBuilder.Entity<Car>(entity =>
        {
            entity.HasKey(e => e.CarId).HasName("PK__car__4C9A0DB316FB2041");

            entity.ToTable("car");

            entity.HasIndex(e => e.LicensePlate, "UQ__car__F72CD56EA7C1AE26").IsUnique();

            entity.Property(e => e.CarId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("car_id");
            entity.Property(e => e.AddressId).HasColumnName("address_id");
            entity.Property(e => e.AverageRating)
                .HasColumnType("decimal(3, 2)")
                .HasColumnName("average_rating");
            entity.Property(e => e.Brand)
                .HasMaxLength(100)
                .HasColumnName("brand");
            entity.Property(e => e.Color)
                .HasMaxLength(50)
                .HasColumnName("color");
            entity.Property(e => e.CreatedAt)
                .HasPrecision(6)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("created_at");
            entity.Property(e => e.FuelType)
                .HasMaxLength(20)
                .HasColumnName("fuel_type");
            entity.Property(e => e.LicensePlate)
                .HasMaxLength(20)
                .HasColumnName("license_plate");
            entity.Property(e => e.MileageKm)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("mileage_km");
            entity.Property(e => e.Model)
                .HasMaxLength(100)
                .HasColumnName("model");
            entity.Property(e => e.NumberOfSeats).HasColumnName("number_of_seats");
            entity.Property(e => e.OwnerAccountId).HasColumnName("owner_account_id");
            entity.Property(e => e.ProductionYear).HasColumnName("production_year");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasColumnName("status");
            entity.Property(e => e.TermOfUse)
                .HasMaxLength(255)
                .IsFixedLength()
                .HasColumnName("term_of_use");
            entity.Property(e => e.TotalRentals).HasColumnName("total_rentals");
            entity.Property(e => e.Transmission)
                .HasMaxLength(20)
                .HasColumnName("transmission");
            entity.Property(e => e.UpdatedAt)
                .HasPrecision(6)
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Address).WithMany(p => p.Cars)
                .HasForeignKey(d => d.AddressId)
                .HasConstraintName("FK__car__address_id__7D439ABD");

            entity.HasOne(d => d.OwnerAccount).WithMany(p => p.Cars)
                .HasForeignKey(d => d.OwnerAccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__car__owner_accou__7E37BEF6");

            entity.HasMany(d => d.Features).WithMany(p => p.Cars)
                .UsingEntity<Dictionary<string, object>>(
                    "CarFeatureLink",
                    r => r.HasOne<CarFeature>().WithMany()
                        .HasForeignKey("FeatureId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__car_featu__featu__02084FDA"),
                    l => l.HasOne<Car>().WithMany()
                        .HasForeignKey("CarId")
                        .HasConstraintName("FK__car_featu__car_i__01142BA1"),
                    j =>
                    {
                        j.HasKey("CarId", "FeatureId").HasName("PK__car_feat__2B0A610EE444F0F6");
                        j.ToTable("car_feature_link");
                        j.IndexerProperty<Guid>("CarId").HasColumnName("car_id");
                        j.IndexerProperty<int>("FeatureId").HasColumnName("feature_id");
                    });
        });

        modelBuilder.Entity<CarCalendar>(entity =>
        {
            entity.HasKey(e => e.CalendarId).HasName("PK__car_cale__584C1344F0D382DB");

            entity.ToTable("car_calendar");

            entity.Property(e => e.CalendarId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("calendar_id");
            entity.Property(e => e.CarId).HasColumnName("car_id");
            entity.Property(e => e.EndTime)
                .HasPrecision(6)
                .HasColumnName("end_time");
            entity.Property(e => e.Reason)
                .HasMaxLength(100)
                .HasColumnName("reason");
            entity.Property(e => e.StartTime)
                .HasPrecision(6)
                .HasColumnName("start_time");

            entity.HasOne(d => d.Car).WithMany(p => p.CarCalendars)
                .HasForeignKey(d => d.CarId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__car_calen__car_i__7F2BE32F");
        });

        modelBuilder.Entity<CarDocument>(entity =>
        {
            entity.HasKey(e => e.DocId).HasName("PK__car_docu__8AD0292402D543BD");

            entity.ToTable("car_document");

            entity.Property(e => e.DocId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("doc_id");
            entity.Property(e => e.CarId).HasColumnName("car_id");
            entity.Property(e => e.DocType)
                .HasMaxLength(50)
                .HasColumnName("doc_type");
            entity.Property(e => e.ExpiresAt).HasColumnName("expires_at");
            entity.Property(e => e.Uri)
                .HasMaxLength(500)
                .HasColumnName("uri");
            entity.Property(e => e.Verified).HasColumnName("verified");
            entity.Property(e => e.VerifiedAt)
                .HasPrecision(6)
                .HasColumnName("verified_at");

            entity.HasOne(d => d.Car).WithMany(p => p.CarDocuments)
                .HasForeignKey(d => d.CarId)
                .HasConstraintName("FK__car_docum__car_i__00200768");
        });

        modelBuilder.Entity<CarFeature>(entity =>
        {
            entity.HasKey(e => e.FeatureId).HasName("PK__car_feat__7906CBD790553D05");

            entity.ToTable("car_feature");

            entity.HasIndex(e => e.Name, "UQ__car_feat__72E12F1BFF3FE1BC").IsUnique();

            entity.Property(e => e.FeatureId).HasColumnName("feature_id");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
        });

        modelBuilder.Entity<CarImage>(entity =>
        {
            entity.HasKey(e => e.ImageId).HasName("PK__car_imag__DC9AC955D81D68B7");

            entity.ToTable("car_image");

            entity.Property(e => e.ImageId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("image_id");
            entity.Property(e => e.CarId).HasColumnName("car_id");
            entity.Property(e => e.ImageType)
                .HasMaxLength(50)
                .HasColumnName("image_type");
            entity.Property(e => e.IsPrimary)
                .HasDefaultValue(false)
                .HasColumnName("is_primary");
            entity.Property(e => e.UploadedAt)
                .HasPrecision(6)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("uploaded_at");
            entity.Property(e => e.Uri)
                .HasMaxLength(500)
                .HasColumnName("uri");

            entity.HasOne(d => d.Car).WithMany(p => p.CarImages)
                .HasForeignKey(d => d.CarId)
                .HasConstraintName("FK__car_image__car_i__02FC7413");
        });

        modelBuilder.Entity<CarPricingPlan>(entity =>
        {
            entity.HasKey(e => e.PlanId).HasName("PK__car_pric__BE9F8F1D3CBF6C37");

            entity.ToTable("car_pricing_plan");

            entity.Property(e => e.PlanId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("plan_id");
            entity.Property(e => e.BasePricePerDayCents)
                .HasColumnType("decimal(19, 2)")
                .HasColumnName("base_price_per_day_cents");
            entity.Property(e => e.CarId).HasColumnName("car_id");
            entity.Property(e => e.DepositCents)
                .HasColumnType("decimal(19, 2)")
                .HasColumnName("deposit_cents");
            entity.Property(e => e.DiscountPercent)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("discount_percent");
            entity.Property(e => e.EffectiveFrom)
                .HasPrecision(6)
                .HasColumnName("effective_from");
            entity.Property(e => e.EffectiveTo)
                .HasPrecision(6)
                .HasColumnName("effective_to");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.IsWeekendOnly)
                .HasDefaultValue(false)
                .HasColumnName("is_weekend_only");
            entity.Property(e => e.KmIncludedDaily).HasColumnName("km_included_daily");
            entity.Property(e => e.MaxDays).HasColumnName("max_days");
            entity.Property(e => e.MinDays).HasColumnName("min_days");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.PricePerExtraKmCents).HasColumnName("price_per_extra_km_cents");

            entity.HasOne(d => d.Car).WithMany(p => p.CarPricingPlans)
                .HasForeignKey(d => d.CarId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__car_prici__car_i__03F0984C");
        });

        modelBuilder.Entity<Promotion>(entity =>
        {
            entity.HasKey(e => e.PromoCode).HasName("PK__promotio__C07E2314BFB4BD7D");

            entity.ToTable("promotion");

            entity.Property(e => e.PromoCode)
                .HasMaxLength(20)
                .HasColumnName("promo_code");
            entity.Property(e => e.DiscountPercent)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("discount_percent");
            entity.Property(e => e.MinDays).HasColumnName("min_days");
            entity.Property(e => e.UsageLimit).HasColumnName("usage_limit");
            entity.Property(e => e.UsedCount)
                .HasDefaultValue(0)
                .HasColumnName("used_count");
            entity.Property(e => e.ValidFrom).HasColumnName("valid_from");
            entity.Property(e => e.ValidTo).HasColumnName("valid_to");
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.ReviewId).HasName("PK__review__60883D90209ED4E4");

            entity.ToTable("review");

            entity.Property(e => e.ReviewId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("review_id");
            entity.Property(e => e.BookingNumber)
                .HasMaxLength(20)
                .HasColumnName("booking_number");
            entity.Property(e => e.Comment).HasColumnName("comment");
            entity.Property(e => e.CreatedAt)
                .HasPrecision(6)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("created_at");
            entity.Property(e => e.FromAccountId).HasColumnName("from_account_id");
            entity.Property(e => e.Rating).HasColumnName("rating");
            entity.Property(e => e.ToAccountId).HasColumnName("to_account_id");

            entity.HasOne(d => d.BookingNumberNavigation).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.BookingNumber)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__review__booking___04E4BC85");

            entity.HasOne(d => d.FromAccount).WithMany(p => p.ReviewFromAccounts)
                .HasForeignKey(d => d.FromAccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__review__from_acc__05D8E0BE");

            entity.HasOne(d => d.ToAccount).WithMany(p => p.ReviewToAccounts)
                .HasForeignKey(d => d.ToAccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__review__to_accou__06CD04F7");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__role__3213E83F80FEFDF9");

            entity.ToTable("role");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.TransactionId).HasName("PK__transact__85C600AF9D64D6DA");

            entity.ToTable("transaction");

            entity.Property(e => e.TransactionId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("transaction_id");
            entity.Property(e => e.AmountCents)
                .HasColumnType("decimal(19, 2)")
                .HasColumnName("amount_cents");
            entity.Property(e => e.BookingNumber)
                .HasMaxLength(20)
                .HasColumnName("booking_number");
            entity.Property(e => e.CreatedAt)
                .HasPrecision(6)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("created_at");
            entity.Property(e => e.Description)
                .HasMaxLength(500)
                .HasColumnName("description");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("processing")
                .HasColumnName("status");
            entity.Property(e => e.Type)
                .HasMaxLength(30)
                .HasColumnName("type");
            entity.Property(e => e.WalletId).HasColumnName("wallet_id");

            entity.HasOne(d => d.BookingNumberNavigation).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.BookingNumber)
                .HasConstraintName("FK__transacti__booki__07C12930");

            entity.HasOne(d => d.Wallet).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.WalletId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__transacti__walle__08B54D69");
        });

        modelBuilder.Entity<UserProfile>(entity =>
        {
            entity.HasKey(e => e.AccountId).HasName("PK__user_pro__46A222CD2299EBAC");

            entity.ToTable("user_profile");

            entity.Property(e => e.AccountId)
                .ValueGeneratedNever()
                .HasColumnName("account_id");
            entity.Property(e => e.AddressId).HasColumnName("address_id");
            entity.Property(e => e.AverageOwnerRating)
                .HasComputedColumnSql("(CONVERT([decimal](3,2),NULL))", true)
                .HasColumnType("decimal(3, 2)")
                .HasColumnName("average_owner_rating");
            entity.Property(e => e.AverageRenterRating)
                .HasComputedColumnSql("(CONVERT([decimal](3,2),NULL))", true)
                .HasColumnType("decimal(3, 2)")
                .HasColumnName("average_renter_rating");
            entity.Property(e => e.CreatedAt)
                .HasPrecision(6)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("created_at");
            entity.Property(e => e.Dob).HasColumnName("dob");
            entity.Property(e => e.DrivingLicenseUri)
                .HasMaxLength(500)
                .HasColumnName("driving_license_uri");
            entity.Property(e => e.DrivingLicenseVerified)
                .HasDefaultValue(false)
                .HasColumnName("driving_license_verified");
            entity.Property(e => e.FullName)
                .HasMaxLength(255)
                .HasColumnName("full_name");
            entity.Property(e => e.NationalId)
                .HasMaxLength(20)
                .HasColumnName("national_id");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(20)
                .HasColumnName("phone_number");
            entity.Property(e => e.UpdatedAt)
                .HasPrecision(6)
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Account).WithOne(p => p.UserProfile)
                .HasForeignKey<UserProfile>(d => d.AccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_user_profile_account");

            entity.HasOne(d => d.Address).WithMany(p => p.UserProfiles)
                .HasForeignKey(d => d.AddressId)
                .HasConstraintName("FK__user_prof__addre__09A971A2");
        });

        modelBuilder.Entity<Wallet>(entity =>
        {
            entity.HasKey(e => e.AccountId).HasName("PK__wallet__46A222CD01B3D270");

            entity.ToTable("wallet");

            entity.Property(e => e.AccountId)
                .ValueGeneratedNever()
                .HasColumnName("account_id");
            entity.Property(e => e.BalanceCents)
                .HasColumnType("decimal(19, 2)")
                .HasColumnName("balance_cents");
            entity.Property(e => e.LockedCents)
                .HasColumnType("decimal(19, 2)")
                .HasColumnName("locked_cents");
            entity.Property(e => e.UpdatedAt)
                .HasPrecision(6)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Account).WithOne(p => p.Wallet)
                .HasForeignKey<Wallet>(d => d.AccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__wallet__account___0B91BA14");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
