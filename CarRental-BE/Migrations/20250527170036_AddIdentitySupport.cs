using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarRental_BE.Migrations
{
    /// <inheritdoc />
    public partial class AddIdentitySupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "role",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NormalizedName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__role__3213E83F5B32EA5A", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "RoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleClaims", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserClaims", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLogins", x => new { x.LoginProvider, x.ProviderKey });
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => new { x.UserId, x.RoleId });
                });

            migrationBuilder.CreateTable(
                name: "UserTokens",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "account",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    password = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    is_active = table.Column<bool>(type: "bit", nullable: false),
                    is_email_verified = table.Column<bool>(type: "bit", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2(6)", precision: 6, nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2(6)", precision: 6, nullable: true),
                    role_id = table.Column<int>(type: "int", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__account__3213E83FC9120251", x => x.id);
                    table.ForeignKey(
                        name: "FK__account__role_id__6FE99F9F",
                        column: x => x.role_id,
                        principalTable: "role",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "car",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    brand = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    model = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    color = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    base_price = table.Column<long>(type: "bigint", nullable: false),
                    deposit = table.Column<long>(type: "bigint", nullable: false),
                    number_of_seats = table.Column<int>(type: "int", nullable: true),
                    production_year = table.Column<int>(type: "int", nullable: true),
                    mileage = table.Column<double>(type: "float", nullable: true),
                    fuel_consumption = table.Column<double>(type: "float", nullable: true),
                    is_gasoline = table.Column<bool>(type: "bit", nullable: true),
                    is_automatic = table.Column<bool>(type: "bit", nullable: true),
                    term_of_use = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    additional_function = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    license_plate = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    house_number_street = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ward = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    district = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    city_province = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    car_image_front = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    car_image_back = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    car_image_left = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    car_image_right = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    insurance_uri = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    insurance_uri_is_verified = table.Column<bool>(type: "bit", nullable: true),
                    registration_paper_uri = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    registration_paper_uri_is_verified = table.Column<bool>(type: "bit", nullable: true),
                    certificate_of_inspection_uri = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    certificate_of_inspection_uri_is_verified = table.Column<bool>(type: "bit", nullable: true),
                    status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    account_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2(6)", precision: 6, nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime2(6)", precision: 6, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__car__3213E83FCD1E1C72", x => x.id);
                    table.ForeignKey(
                        name: "FK__car__account_id__72C60C4A",
                        column: x => x.account_id,
                        principalTable: "account",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "user_profile",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    full_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    dob = table.Column<DateOnly>(type: "date", nullable: true),
                    phone_number = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    national_id = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    driving_license_uri = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    house_number_street = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ward = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    district = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    city_province = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__user_pro__3213E83FA9CD380B", x => x.id);
                    table.ForeignKey(
                        name: "FK_user_profile_account",
                        column: x => x.id,
                        principalTable: "account",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "wallet",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    balance = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__wallet__3213E83F0E1EA4EF", x => x.id);
                    table.ForeignKey(
                        name: "FK_wallet_account",
                        column: x => x.id,
                        principalTable: "account",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "booking",
                columns: table => new
                {
                    booking_number = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    car_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    driver_full_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    driver_dob = table.Column<DateOnly>(type: "date", nullable: true),
                    driver_email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    driver_phone_number = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    driver_national_id = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    driver_driving_license_uri = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    driver_house_number_street = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    driver_ward = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    driver_district = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    driver_city_province = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    base_price = table.Column<long>(type: "bigint", nullable: true),
                    deposit = table.Column<long>(type: "bigint", nullable: true),
                    pick_up_location = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    drop_off_location = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    pick_up_time = table.Column<DateTime>(type: "datetime2(6)", precision: 6, nullable: true),
                    drop_off_time = table.Column<DateTime>(type: "datetime2(6)", precision: 6, nullable: true),
                    payment_type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2(6)", precision: 6, nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime2(6)", precision: 6, nullable: true),
                    account_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__booking__3A30D2BD1203780A", x => x.booking_number);
                    table.ForeignKey(
                        name: "FK__booking__car_id__70DDC3D8",
                        column: x => x.car_id,
                        principalTable: "car",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_booking_account",
                        column: x => x.account_id,
                        principalTable: "account",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "feedback",
                columns: table => new
                {
                    booking_number = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    rating = table.Column<int>(type: "int", nullable: true),
                    create_at = table.Column<DateTime>(type: "datetime2(6)", precision: 6, nullable: true),
                    update_at = table.Column<DateTime>(type: "datetime2(6)", precision: 6, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__feedback__3A30D2BDCACAA59D", x => x.booking_number);
                    table.ForeignKey(
                        name: "FK__feedback__bookin__73BA3083",
                        column: x => x.booking_number,
                        principalTable: "booking",
                        principalColumn: "booking_number");
                });

            migrationBuilder.CreateTable(
                name: "transaction",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    wallet_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    booking_number = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    amount = table.Column<long>(type: "bigint", nullable: false),
                    car_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    message = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2(6)", precision: 6, nullable: true),
                    status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__transact__3213E83FA1431324", x => x.id);
                    table.ForeignKey(
                        name: "FK__transacti__booki__74AE54BC",
                        column: x => x.booking_number,
                        principalTable: "booking",
                        principalColumn: "booking_number");
                    table.ForeignKey(
                        name: "FK__transacti__walle__75A278F5",
                        column: x => x.wallet_id,
                        principalTable: "wallet",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_account_role_id",
                table: "account",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "UQ__account__AB6E6164C0CD80B1",
                table: "account",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_booking_account_id",
                table: "booking",
                column: "account_id");

            migrationBuilder.CreateIndex(
                name: "IX_booking_car_id",
                table: "booking",
                column: "car_id");

            migrationBuilder.CreateIndex(
                name: "IX_car_account_id",
                table: "car",
                column: "account_id");

            migrationBuilder.CreateIndex(
                name: "IX_transaction_booking_number",
                table: "transaction",
                column: "booking_number");

            migrationBuilder.CreateIndex(
                name: "IX_transaction_wallet_id",
                table: "transaction",
                column: "wallet_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "feedback");

            migrationBuilder.DropTable(
                name: "RoleClaims");

            migrationBuilder.DropTable(
                name: "transaction");

            migrationBuilder.DropTable(
                name: "user_profile");

            migrationBuilder.DropTable(
                name: "UserClaims");

            migrationBuilder.DropTable(
                name: "UserLogins");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "UserTokens");

            migrationBuilder.DropTable(
                name: "booking");

            migrationBuilder.DropTable(
                name: "wallet");

            migrationBuilder.DropTable(
                name: "car");

            migrationBuilder.DropTable(
                name: "account");

            migrationBuilder.DropTable(
                name: "role");
        }
    }
}
