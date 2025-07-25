﻿// <auto-generated />
using System;
using CarRental_BE.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace CarRental_BE.Migrations
{
    [DbContext(typeof(CarRentalContext))]
    partial class CarRentalContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("CarRental_BE.Models.Entities.Account", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("id")
                        .HasDefaultValueSql("(newid())");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("ConcurrencyStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedAt")
                        .HasPrecision(6)
                        .HasColumnType("datetime2(6)")
                        .HasColumnName("created_at");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)")
                        .HasColumnName("email");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit")
                        .HasColumnName("is_active");

                    b.Property<bool>("IsEmailVerified")
                        .HasColumnType("bit")
                        .HasColumnName("is_email_verified");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("NormalizedEmail")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NormalizedUserName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)")
                        .HasColumnName("password");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<int>("RoleId")
                        .HasColumnType("int")
                        .HasColumnName("role_id");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasPrecision(6)
                        .HasColumnType("datetime2(6)")
                        .HasColumnName("updated_at");

                    b.Property<string>("UserName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id")
                        .HasName("PK__account__3213E83FC9120251");

                    b.HasIndex("RoleId");

                    b.HasIndex(new[] { "Email" }, "UQ__account__AB6E6164C0CD80B1")
                        .IsUnique();

                    b.ToTable("account", (string)null);
                });

            modelBuilder.Entity("CarRental_BE.Models.Entities.Booking", b =>
                {
                    b.Property<string>("BookingNumber")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)")
                        .HasColumnName("booking_number");

                    b.Property<Guid?>("AccountId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("account_id");

                    b.Property<long?>("BasePrice")
                        .HasColumnType("bigint")
                        .HasColumnName("base_price");

                    b.Property<Guid>("CarId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("car_id");

                    b.Property<DateTime?>("CreatedAt")
                        .HasPrecision(6)
                        .HasColumnType("datetime2(6)")
                        .HasColumnName("created_at");

                    b.Property<long?>("Deposit")
                        .HasColumnType("bigint")
                        .HasColumnName("deposit");

                    b.Property<string>("DriverCityProvince")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)")
                        .HasColumnName("driver_city_province");

                    b.Property<string>("DriverDistrict")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)")
                        .HasColumnName("driver_district");

                    b.Property<DateOnly?>("DriverDob")
                        .HasColumnType("date")
                        .HasColumnName("driver_dob");

                    b.Property<string>("DriverDrivingLicenseUri")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)")
                        .HasColumnName("driver_driving_license_uri");

                    b.Property<string>("DriverEmail")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)")
                        .HasColumnName("driver_email");

                    b.Property<string>("DriverFullName")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)")
                        .HasColumnName("driver_full_name");

                    b.Property<string>("DriverHouseNumberStreet")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)")
                        .HasColumnName("driver_house_number_street");

                    b.Property<string>("DriverNationalId")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)")
                        .HasColumnName("driver_national_id");

                    b.Property<string>("DriverPhoneNumber")
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)")
                        .HasColumnName("driver_phone_number");

                    b.Property<string>("DriverWard")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)")
                        .HasColumnName("driver_ward");

                    b.Property<string>("DropOffLocation")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)")
                        .HasColumnName("drop_off_location");

                    b.Property<DateTime?>("DropOffTime")
                        .HasPrecision(6)
                        .HasColumnType("datetime2(6)")
                        .HasColumnName("drop_off_time");

                    b.Property<string>("PaymentType")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)")
                        .HasColumnName("payment_type");

                    b.Property<string>("PickUpLocation")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)")
                        .HasColumnName("pick_up_location");

                    b.Property<DateTime?>("PickUpTime")
                        .HasPrecision(6)
                        .HasColumnType("datetime2(6)")
                        .HasColumnName("pick_up_time");

                    b.Property<string>("Status")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)")
                        .HasColumnName("status");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasPrecision(6)
                        .HasColumnType("datetime2(6)")
                        .HasColumnName("updated_at");

                    b.HasKey("BookingNumber")
                        .HasName("PK__booking__3A30D2BD1203780A");

                    b.HasIndex("AccountId");

                    b.HasIndex("CarId");

                    b.ToTable("booking", (string)null);
                });

            modelBuilder.Entity("CarRental_BE.Models.Entities.Car", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("id")
                        .HasDefaultValueSql("(newid())");

                    b.Property<Guid>("AccountId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("account_id");

                    b.Property<string>("AdditionalFunction")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)")
                        .HasColumnName("additional_function");

                    b.Property<long>("BasePrice")
                        .HasColumnType("bigint")
                        .HasColumnName("base_price");

                    b.Property<string>("Brand")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)")
                        .HasColumnName("brand");

                    b.Property<string>("CarImageBack")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)")
                        .HasColumnName("car_image_back");

                    b.Property<string>("CarImageFront")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)")
                        .HasColumnName("car_image_front");

                    b.Property<string>("CarImageLeft")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)")
                        .HasColumnName("car_image_left");

                    b.Property<string>("CarImageRight")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)")
                        .HasColumnName("car_image_right");

                    b.Property<string>("CertificateOfInspectionUri")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)")
                        .HasColumnName("certificate_of_inspection_uri");

                    b.Property<bool?>("CertificateOfInspectionUriIsVerified")
                        .HasColumnType("bit")
                        .HasColumnName("certificate_of_inspection_uri_is_verified");

                    b.Property<string>("CityProvince")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)")
                        .HasColumnName("city_province");

                    b.Property<string>("Color")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)")
                        .HasColumnName("color");

                    b.Property<DateTime?>("CreatedAt")
                        .HasPrecision(6)
                        .HasColumnType("datetime2(6)")
                        .HasColumnName("created_at");

                    b.Property<long>("Deposit")
                        .HasColumnType("bigint")
                        .HasColumnName("deposit");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("description");

                    b.Property<string>("District")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)")
                        .HasColumnName("district");

                    b.Property<double?>("FuelConsumption")
                        .HasColumnType("float")
                        .HasColumnName("fuel_consumption");

                    b.Property<string>("HouseNumberStreet")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)")
                        .HasColumnName("house_number_street");

                    b.Property<string>("InsuranceUri")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)")
                        .HasColumnName("insurance_uri");

                    b.Property<bool?>("InsuranceUriIsVerified")
                        .HasColumnType("bit")
                        .HasColumnName("insurance_uri_is_verified");

                    b.Property<bool?>("IsAutomatic")
                        .HasColumnType("bit")
                        .HasColumnName("is_automatic");

                    b.Property<bool?>("IsGasoline")
                        .HasColumnType("bit")
                        .HasColumnName("is_gasoline");

                    b.Property<string>("LicensePlate")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)")
                        .HasColumnName("license_plate");

                    b.Property<double?>("Mileage")
                        .HasColumnType("float")
                        .HasColumnName("mileage");

                    b.Property<string>("Model")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)")
                        .HasColumnName("model");

                    b.Property<int?>("NumberOfSeats")
                        .HasColumnType("int")
                        .HasColumnName("number_of_seats");

                    b.Property<int?>("ProductionYear")
                        .HasColumnType("int")
                        .HasColumnName("production_year");

                    b.Property<string>("RegistrationPaperUri")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)")
                        .HasColumnName("registration_paper_uri");

                    b.Property<bool?>("RegistrationPaperUriIsVerified")
                        .HasColumnType("bit")
                        .HasColumnName("registration_paper_uri_is_verified");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)")
                        .HasColumnName("status");

                    b.Property<string>("TermOfUse")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)")
                        .HasColumnName("term_of_use");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasPrecision(6)
                        .HasColumnType("datetime2(6)")
                        .HasColumnName("updated_at");

                    b.Property<string>("Ward")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)")
                        .HasColumnName("ward");

                    b.HasKey("Id")
                        .HasName("PK__car__3213E83FCD1E1C72");

                    b.HasIndex("AccountId");

                    b.ToTable("car", (string)null);
                });

            modelBuilder.Entity("CarRental_BE.Models.Entities.Feedback", b =>
                {
                    b.Property<string>("BookingNumber")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)")
                        .HasColumnName("booking_number");

                    b.Property<string>("Comment")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("comment");

                    b.Property<DateTime?>("CreateAt")
                        .HasPrecision(6)
                        .HasColumnType("datetime2(6)")
                        .HasColumnName("create_at");

                    b.Property<int?>("Rating")
                        .HasColumnType("int")
                        .HasColumnName("rating");

                    b.Property<DateTime?>("UpdateAt")
                        .HasPrecision(6)
                        .HasColumnType("datetime2(6)")
                        .HasColumnName("update_at");

                    b.HasKey("BookingNumber")
                        .HasName("PK__feedback__3A30D2BDCACAA59D");

                    b.ToTable("feedback", (string)null);
                });

            modelBuilder.Entity("CarRental_BE.Models.Entities.Role", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ConcurrencyStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)")
                        .HasColumnName("name");

                    b.Property<string>("NormalizedName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id")
                        .HasName("PK__role__3213E83F5B32EA5A");

                    b.ToTable("role", (string)null);
                });

            modelBuilder.Entity("CarRental_BE.Models.Entities.Transaction", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("id")
                        .HasDefaultValueSql("(newid())");

                    b.Property<long>("Amount")
                        .HasColumnType("bigint")
                        .HasColumnName("amount");

                    b.Property<string>("BookingNumber")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)")
                        .HasColumnName("booking_number");

                    b.Property<string>("CarName")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)")
                        .HasColumnName("car_name");

                    b.Property<DateTime?>("CreatedAt")
                        .HasPrecision(6)
                        .HasColumnType("datetime2(6)")
                        .HasColumnName("created_at");

                    b.Property<string>("Message")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)")
                        .HasColumnName("message");

                    b.Property<string>("Status")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)")
                        .HasColumnName("status");

                    b.Property<string>("Type")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)")
                        .HasColumnName("type");

                    b.Property<Guid>("WalletId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("wallet_id");

                    b.HasKey("Id")
                        .HasName("PK__transact__3213E83FA1431324");

                    b.HasIndex("BookingNumber");

                    b.HasIndex("WalletId");

                    b.ToTable("transaction", (string)null);
                });

            modelBuilder.Entity("CarRental_BE.Models.Entities.UserProfile", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("id")
                        .HasDefaultValueSql("(newid())");

                    b.Property<string>("CityProvince")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)")
                        .HasColumnName("city_province");

                    b.Property<string>("District")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)")
                        .HasColumnName("district");

                    b.Property<DateOnly?>("Dob")
                        .HasColumnType("date")
                        .HasColumnName("dob");

                    b.Property<string>("DrivingLicenseUri")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)")
                        .HasColumnName("driving_license_uri");

                    b.Property<string>("FullName")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)")
                        .HasColumnName("full_name");

                    b.Property<string>("HouseNumberStreet")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)")
                        .HasColumnName("house_number_street");

                    b.Property<string>("NationalId")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)")
                        .HasColumnName("national_id");

                    b.Property<string>("PhoneNumber")
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)")
                        .HasColumnName("phone_number");

                    b.Property<string>("Ward")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)")
                        .HasColumnName("ward");

                    b.HasKey("Id")
                        .HasName("PK__user_pro__3213E83FA9CD380B");

                    b.ToTable("user_profile", (string)null);
                });

            modelBuilder.Entity("CarRental_BE.Models.Entities.Wallet", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("id")
                        .HasDefaultValueSql("(newid())");

                    b.Property<long>("Balance")
                        .HasColumnType("bigint")
                        .HasColumnName("balance");

                    b.HasKey("Id")
                        .HasName("PK__wallet__3213E83F0E1EA4EF");

                    b.ToTable("wallet", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<System.Guid>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("RoleId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.ToTable("RoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<System.Guid>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.ToTable("UserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<System.Guid>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.ToTable("UserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<System.Guid>", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("RoleId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("UserId", "RoleId");

                    b.ToTable("UserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<System.Guid>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.ToTable("UserTokens");
                });

            modelBuilder.Entity("CarRental_BE.Models.Entities.Account", b =>
                {
                    b.HasOne("CarRental_BE.Models.Entities.Role", "Role")
                        .WithMany("Accounts")
                        .HasForeignKey("RoleId")
                        .IsRequired()
                        .HasConstraintName("FK__account__role_id__6FE99F9F");

                    b.Navigation("Role");
                });

            modelBuilder.Entity("CarRental_BE.Models.Entities.Booking", b =>
                {
                    b.HasOne("CarRental_BE.Models.Entities.Account", "Account")
                        .WithMany("Bookings")
                        .HasForeignKey("AccountId")
                        .HasConstraintName("FK_booking_account");

                    b.HasOne("CarRental_BE.Models.Entities.Car", "Car")
                        .WithMany("Bookings")
                        .HasForeignKey("CarId")
                        .IsRequired()
                        .HasConstraintName("FK__booking__car_id__70DDC3D8");

                    b.Navigation("Account");

                    b.Navigation("Car");
                });

            modelBuilder.Entity("CarRental_BE.Models.Entities.Car", b =>
                {
                    b.HasOne("CarRental_BE.Models.Entities.Account", "Account")
                        .WithMany("Cars")
                        .HasForeignKey("AccountId")
                        .IsRequired()
                        .HasConstraintName("FK__car__account_id__72C60C4A");

                    b.Navigation("Account");
                });

            modelBuilder.Entity("CarRental_BE.Models.Entities.Feedback", b =>
                {
                    b.HasOne("CarRental_BE.Models.Entities.Booking", "BookingNumberNavigation")
                        .WithOne("Feedback")
                        .HasForeignKey("CarRental_BE.Models.Entities.Feedback", "BookingNumber")
                        .IsRequired()
                        .HasConstraintName("FK__feedback__bookin__73BA3083");

                    b.Navigation("BookingNumberNavigation");
                });

            modelBuilder.Entity("CarRental_BE.Models.Entities.Transaction", b =>
                {
                    b.HasOne("CarRental_BE.Models.Entities.Booking", "BookingNumberNavigation")
                        .WithMany("Transactions")
                        .HasForeignKey("BookingNumber")
                        .HasConstraintName("FK__transacti__booki__74AE54BC");

                    b.HasOne("CarRental_BE.Models.Entities.Wallet", "Wallet")
                        .WithMany("Transactions")
                        .HasForeignKey("WalletId")
                        .IsRequired()
                        .HasConstraintName("FK__transacti__walle__75A278F5");

                    b.Navigation("BookingNumberNavigation");

                    b.Navigation("Wallet");
                });

            modelBuilder.Entity("CarRental_BE.Models.Entities.UserProfile", b =>
                {
                    b.HasOne("CarRental_BE.Models.Entities.Account", "IdNavigation")
                        .WithOne("UserProfile")
                        .HasForeignKey("CarRental_BE.Models.Entities.UserProfile", "Id")
                        .IsRequired()
                        .HasConstraintName("FK_user_profile_account");

                    b.Navigation("IdNavigation");
                });

            modelBuilder.Entity("CarRental_BE.Models.Entities.Wallet", b =>
                {
                    b.HasOne("CarRental_BE.Models.Entities.Account", "IdNavigation")
                        .WithOne("Wallet")
                        .HasForeignKey("CarRental_BE.Models.Entities.Wallet", "Id")
                        .IsRequired()
                        .HasConstraintName("FK_wallet_account");

                    b.Navigation("IdNavigation");
                });

            modelBuilder.Entity("CarRental_BE.Models.Entities.Account", b =>
                {
                    b.Navigation("Bookings");

                    b.Navigation("Cars");

                    b.Navigation("UserProfile");

                    b.Navigation("Wallet");
                });

            modelBuilder.Entity("CarRental_BE.Models.Entities.Booking", b =>
                {
                    b.Navigation("Feedback");

                    b.Navigation("Transactions");
                });

            modelBuilder.Entity("CarRental_BE.Models.Entities.Car", b =>
                {
                    b.Navigation("Bookings");
                });

            modelBuilder.Entity("CarRental_BE.Models.Entities.Role", b =>
                {
                    b.Navigation("Accounts");
                });

            modelBuilder.Entity("CarRental_BE.Models.Entities.Wallet", b =>
                {
                    b.Navigation("Transactions");
                });
#pragma warning restore 612, 618
        }
    }
}
