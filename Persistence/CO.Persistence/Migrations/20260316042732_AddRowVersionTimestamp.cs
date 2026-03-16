using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CO.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddRowVersionTimestamp : Migration
    {
        /// <inheritdoc />
        //protected override void Up(MigrationBuilder migrationBuilder)
        //{
        //    migrationBuilder.DropColumn(name: "RowVersion", table: "Directors");
        //    migrationBuilder.AlterColumn<byte[]>(
        //        name: "RowVersion",
        //        table: "Directors",
        //        type: "rowversion",
        //        rowVersion: true,
        //        nullable: false,
        //        oldClrType: typeof(byte[]),
        //        oldType: "varbinary(max)");

        //    migrationBuilder.DropColumn(name: "RowVersion", table: "Clients");
        //    migrationBuilder.AlterColumn<byte[]>(
        //        name: "RowVersion",
        //        table: "Clients",
        //        type: "rowversion",
        //        rowVersion: true,
        //        nullable: false,
        //        oldClrType: typeof(byte[]),
        //        oldType: "varbinary(max)");
        //}

        ///// <inheritdoc />
        //protected override void Down(MigrationBuilder migrationBuilder)
        //{
        //    migrationBuilder.DropColumn(name: "RowVersion", table: "Directors");
        //    migrationBuilder.AlterColumn<byte[]>(
        //        name: "RowVersion",
        //        table: "Directors",
        //        type: "varbinary(max)",
        //        nullable: false,
        //        oldClrType: typeof(byte[]),
        //        oldType: "rowversion",
        //        oldRowVersion: true);

        //    migrationBuilder.DropColumn(name: "RowVersion", table: "Clients");
        //    migrationBuilder.AlterColumn<byte[]>(
        //        name: "RowVersion",
        //        table: "Clients",
        //        type: "varbinary(max)",
        //        nullable: false,
        //        oldClrType: typeof(byte[]),
        //        oldType: "rowversion",
        //        oldRowVersion: true);
        //}

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "RowVersion", table: "Directors");
            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Directors",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.DropColumn(name: "RowVersion", table: "Clients");
            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Clients",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.DropColumn(name: "RowVersion", table: "StaffMembers");
            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "StaffMembers",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "RowVersion", table: "Directors");
            migrationBuilder.AddColumn<byte[]>(name: "RowVersion", table: "Directors",
                type: "varbinary(max)", nullable: false, defaultValue: new byte[0]);

            migrationBuilder.DropColumn(name: "RowVersion", table: "Clients");
            migrationBuilder.AddColumn<byte[]>(name: "RowVersion", table: "Clients",
                type: "varbinary(max)", nullable: false, defaultValue: new byte[0]);

            migrationBuilder.DropColumn(name: "RowVersion", table: "StaffMembers");
            migrationBuilder.AddColumn<byte[]>(name: "RowVersion", table: "StaffMembers",
                type: "varbinary(max)", nullable: false, defaultValue: new byte[0]);
        }
    }
}
