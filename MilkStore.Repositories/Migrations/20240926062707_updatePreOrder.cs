using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MilkStore.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class updatePreOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PreoderDate",
                table: "PreOrders");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "PreoderDate",
                table: "PreOrders",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
