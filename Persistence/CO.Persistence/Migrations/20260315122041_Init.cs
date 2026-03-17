using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CO.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StaffMembers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    StaffNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Department = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaffMembers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Clients",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClientNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ClientName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClientType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SegmentType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SubSegmentType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    OpenedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    RelationshipManagerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyName = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    LineOfBusiness = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LineOfBusinessMoreInfo = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NatureOfBusiness = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    IdentificationType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    RegistrationNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DateOfRegistration = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    RegisteredAt = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    RegisteredOffice = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    BusinessStartedYear = table.Column<int>(type: "int", nullable: true),
                    NumberOfEmployees = table.Column<int>(type: "int", nullable: true),
                    Comments = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Website = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    TINNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CorporateDetail_ClientClassification = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResidentialAddress = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    BusinessAddress = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    OfficeAddress = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    MailingAddress = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    HomeCountryAddress = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    AddressLine2 = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Country = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Region = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Ward = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    District = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Street = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ZipCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PhoneHome = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    PhoneWork = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Mobile = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    FaxNo = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    LandMark = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    EmailId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CanSendGreetings = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CanSendAssociateSpecialOffer = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CanSendOurSpecialOffers = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    StatementOnline = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    MobileAlert = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "varbinary(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Clients_StaffMembers_RelationshipManagerId",
                        column: x => x.RelationshipManagerId,
                        principalTable: "StaffMembers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Directors",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    RelationType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IdentificationType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IdentificationNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    SharePercentage = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    DateAdded = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "varbinary(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Directors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Directors_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "StaffMembers",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "Department", "FirstName", "IsDeleted", "LastName", "StaffNumber", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("11111111-0000-0000-0000-000000000001"), new DateTimeOffset(new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "SYSTEM", null, null, "Corporate Banking", "James", false, "Ochieng", "RM001", null, null },
                    { new Guid("11111111-0000-0000-0000-000000000002"), new DateTimeOffset(new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "SYSTEM", null, null, "Corporate Banking", "Amina", false, "Hassan", "RM002", null, null },
                    { new Guid("11111111-0000-0000-0000-000000000003"), new DateTimeOffset(new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "SYSTEM", null, null, "SME Banking", "Peter", false, "Mwangi", "RM003", null, null },
                    { new Guid("11111111-0000-0000-0000-000000000004"), new DateTimeOffset(new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "SYSTEM", null, null, "Corporate Banking", "Grace", false, "Wanjiku", "RM004", null, null },
                    { new Guid("11111111-0000-0000-0000-000000000005"), new DateTimeOffset(new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "SYSTEM", null, null, "Retail Banking", "David", false, "Kamau", "RM005", null, null },
                    { new Guid("11111111-0000-0000-0000-000000000006"), new DateTimeOffset(new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "SYSTEM", null, null, "SME Banking", "Fatuma", false, "Ally", "RM006", null, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Clients_ClientNumber",
                table: "Clients",
                column: "ClientNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Clients_RegistrationNumber",
                table: "Clients",
                column: "RegistrationNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Clients_RelationshipManagerId",
                table: "Clients",
                column: "RelationshipManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_Directors_ClientId_IdentificationNumber",
                table: "Directors",
                columns: new[] { "ClientId", "IdentificationNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StaffMembers_StaffNumber",
                table: "StaffMembers",
                column: "StaffNumber",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Directors");

            migrationBuilder.DropTable(
                name: "Clients");

            migrationBuilder.DropTable(
                name: "StaffMembers");
        }
    }
}
