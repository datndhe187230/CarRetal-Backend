using System;
using System.Collections.Generic;
using CarRental_BE.Models.Entities;
using Microsoft.EntityFrameworkCore;

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

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<Car> Cars { get; set; }

    public virtual DbSet<Feedback> Feedbacks { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Transaction> Transactions { get; set; }

    public virtual DbSet<UserProfile> UserProfiles { get; set; }

    public virtual DbSet<Wallet> Wallets { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=car-rental.cl22ku0yy1qy.ap-southeast-2.rds.amazonaws.com;Database=CarRental;User Id=admin;Password=quanquan3007;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__account__3213E83F810ED317");

            entity.ToTable("account");

            entity.HasIndex(e => e.Email, "UQ__account__AB6E6164AB74690D").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasPrecision(6)
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.IsEmailVerified).HasColumnName("is_email_verified");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.UpdatedAt)
                .HasPrecision(6)
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Role).WithMany(p => p.Accounts)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__account__role_id__49C3F6B7");
        });

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.BookingNumber).HasName("PK__booking__3A30D2BD9040607F");

            entity.ToTable("booking");

            entity.Property(e => e.BookingNumber)
                .HasMaxLength(255)
                .HasColumnName("booking_number");
            entity.Property(e => e.AccountId).HasColumnName("account_id");
            entity.Property(e => e.BasePrice).HasColumnName("base_price");
            entity.Property(e => e.CarId).HasColumnName("car_id");
            entity.Property(e => e.CreatedAt)
                .HasPrecision(6)
                .HasColumnName("created_at");
            entity.Property(e => e.Deposit).HasColumnName("deposit");
            entity.Property(e => e.DriverCityProvince)
                .HasMaxLength(255)
                .HasColumnName("driver_city_province");
            entity.Property(e => e.DriverDistrict)
                .HasMaxLength(255)
                .HasColumnName("driver_district");
            entity.Property(e => e.DriverDob).HasColumnName("driver_dob");
            entity.Property(e => e.DriverDrivingLicenseUri)
                .HasMaxLength(255)
                .HasColumnName("driver_driving_license_uri");
            entity.Property(e => e.DriverEmail)
                .HasMaxLength(255)
                .HasColumnName("driver_email");
            entity.Property(e => e.DriverFullName)
                .HasMaxLength(255)
                .HasColumnName("driver_full_name");
            entity.Property(e => e.DriverHouseNumberStreet)
                .HasMaxLength(255)
                .HasColumnName("driver_house_number_street");
            entity.Property(e => e.DriverNationalId)
                .HasMaxLength(255)
                .HasColumnName("driver_national_id");
            entity.Property(e => e.DriverPhoneNumber)
                .HasMaxLength(20)
                .HasColumnName("driver_phone_number");
            entity.Property(e => e.DriverWard)
                .HasMaxLength(255)
                .HasColumnName("driver_ward");
            entity.Property(e => e.DropOffLocation)
                .HasMaxLength(255)
                .HasColumnName("drop_off_location");
            entity.Property(e => e.DropOffTime)
                .HasPrecision(6)
                .HasColumnName("drop_off_time");
            entity.Property(e => e.PaymentType)
                .HasMaxLength(50)
                .HasColumnName("payment_type");
            entity.Property(e => e.PickUpLocation)
                .HasMaxLength(255)
                .HasColumnName("pick_up_location");
            entity.Property(e => e.PickUpTime)
                .HasPrecision(6)
                .HasColumnName("pick_up_time");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasColumnName("status");
            entity.Property(e => e.UpdatedAt)
                .HasPrecision(6)
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Account).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.AccountId)
                .HasConstraintName("FK_booking_account");

            entity.HasOne(d => d.Car).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.CarId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__booking__car_id__4AB81AF0");
        });

        modelBuilder.Entity<Car>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__car__3213E83F6C48D400");

            entity.ToTable("car");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.AccountId).HasColumnName("account_id");
            entity.Property(e => e.AdditionalFunction)
                .HasMaxLength(255)
                .HasColumnName("additional_function");
            entity.Property(e => e.BasePrice).HasColumnName("base_price");
            entity.Property(e => e.Brand)
                .HasMaxLength(255)
                .HasColumnName("brand");
            entity.Property(e => e.CarImageBack)
                .HasMaxLength(255)
                .HasColumnName("car_image_back");
            entity.Property(e => e.CarImageFront)
                .HasMaxLength(255)
                .HasColumnName("car_image_front");
            entity.Property(e => e.CarImageLeft)
                .HasMaxLength(255)
                .HasColumnName("car_image_left");
            entity.Property(e => e.CarImageRight)
                .HasMaxLength(255)
                .HasColumnName("car_image_right");
            entity.Property(e => e.CertificateOfInspectionUri)
                .HasMaxLength(255)
                .HasColumnName("certificate_of_inspection_uri");
            entity.Property(e => e.CertificateOfInspectionUriIsVerified).HasColumnName("certificate_of_inspection_uri_is_verified");
            entity.Property(e => e.CityProvince)
                .HasMaxLength(255)
                .HasColumnName("city_province");
            entity.Property(e => e.Color)
                .HasMaxLength(255)
                .HasColumnName("color");
            entity.Property(e => e.CreatedAt)
                .HasPrecision(6)
                .HasColumnName("created_at");
            entity.Property(e => e.Deposit).HasColumnName("deposit");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.District)
                .HasMaxLength(255)
                .HasColumnName("district");
            entity.Property(e => e.FuelConsumption).HasColumnName("fuel_consumption");
            entity.Property(e => e.HouseNumberStreet)
                .HasMaxLength(255)
                .HasColumnName("house_number_street");
            entity.Property(e => e.InsuranceUri)
                .HasMaxLength(255)
                .HasColumnName("insurance_uri");
            entity.Property(e => e.InsuranceUriIsVerified).HasColumnName("insurance_uri_is_verified");
            entity.Property(e => e.IsAutomatic).HasColumnName("is_automatic");
            entity.Property(e => e.IsGasoline).HasColumnName("is_gasoline");
            entity.Property(e => e.LicensePlate)
                .HasMaxLength(255)
                .HasColumnName("license_plate");
            entity.Property(e => e.Mileage).HasColumnName("mileage");
            entity.Property(e => e.Model)
                .HasMaxLength(255)
                .HasColumnName("model");
            entity.Property(e => e.NumberOfSeats).HasColumnName("number_of_seats");
            entity.Property(e => e.ProductionYear).HasColumnName("production_year");
            entity.Property(e => e.RegistrationPaperUri)
                .HasMaxLength(255)
                .HasColumnName("registration_paper_uri");
            entity.Property(e => e.RegistrationPaperUriIsVerified).HasColumnName("registration_paper_uri_is_verified");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasColumnName("status");
            entity.Property(e => e.TermOfUse)
                .HasMaxLength(255)
                .HasColumnName("term_of_use");
            entity.Property(e => e.UpdatedAt)
                .HasPrecision(6)
                .HasColumnName("updated_at");
            entity.Property(e => e.Ward)
                .HasMaxLength(255)
                .HasColumnName("ward");

            entity.HasOne(d => d.Account).WithMany(p => p.Cars)
                .HasForeignKey(d => d.AccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__car__account_id__4CA06362");
        });

        modelBuilder.Entity<Feedback>(entity =>
        {
            entity.HasKey(e => e.BookingNumber).HasName("PK__feedback__3A30D2BDC5F97129");

            entity.ToTable("feedback");

            entity.Property(e => e.BookingNumber)
                .HasMaxLength(255)
                .HasColumnName("booking_number");
            entity.Property(e => e.Comment).HasColumnName("comment");
            entity.Property(e => e.CreateAt)
                .HasPrecision(6)
                .HasColumnName("create_at");
            entity.Property(e => e.Rating).HasColumnName("rating");
            entity.Property(e => e.UpdateAt)
                .HasPrecision(6)
                .HasColumnName("update_at");

            entity.HasOne(d => d.BookingNumberNavigation).WithOne(p => p.Feedback)
                .HasForeignKey<Feedback>(d => d.BookingNumber)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__feedback__bookin__4D94879B");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__role__3213E83FF9027B9A");

            entity.ToTable("role");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__transact__3213E83F1039FE38");

            entity.ToTable("transaction");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.Amount).HasColumnName("amount");
            entity.Property(e => e.BookingNumber)
                .HasMaxLength(255)
                .HasColumnName("booking_number");
            entity.Property(e => e.CarName)
                .HasMaxLength(255)
                .HasColumnName("car_name");
            entity.Property(e => e.CreatedAt)
                .HasPrecision(6)
                .HasColumnName("created_at");
            entity.Property(e => e.Message)
                .HasMaxLength(255)
                .HasColumnName("message");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasColumnName("status");
            entity.Property(e => e.Type)
                .HasMaxLength(50)
                .HasColumnName("type");
            entity.Property(e => e.WalletId).HasColumnName("wallet_id");

            entity.HasOne(d => d.BookingNumberNavigation).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.BookingNumber)
                .HasConstraintName("FK__transacti__booki__4E88ABD4");

            entity.HasOne(d => d.Wallet).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.WalletId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__transacti__walle__4F7CD00D");
        });

        modelBuilder.Entity<UserProfile>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__user_pro__3213E83FFB7EA4EF");

            entity.ToTable("user_profile");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.CityProvince)
                .HasMaxLength(255)
                .HasColumnName("city_province");
            entity.Property(e => e.District)
                .HasMaxLength(255)
                .HasColumnName("district");
            entity.Property(e => e.Dob).HasColumnName("dob");
            entity.Property(e => e.DrivingLicenseUri)
                .HasMaxLength(255)
                .HasColumnName("driving_license_uri");
            entity.Property(e => e.FullName)
                .HasMaxLength(255)
                .HasColumnName("full_name");
            entity.Property(e => e.HouseNumberStreet)
                .HasMaxLength(255)
                .HasColumnName("house_number_street");
            entity.Property(e => e.NationalId)
                .HasMaxLength(255)
                .HasColumnName("national_id");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(20)
                .HasColumnName("phone_number");
            entity.Property(e => e.Ward)
                .HasMaxLength(255)
                .HasColumnName("ward");

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.UserProfile)
                .HasForeignKey<UserProfile>(d => d.Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_user_profile_account");
        });

        modelBuilder.Entity<Wallet>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__wallet__3213E83F6334CC0B");

            entity.ToTable("wallet");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.Balance).HasColumnName("balance");

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.Wallet)
                .HasForeignKey<Wallet>(d => d.Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_wallet_account");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
