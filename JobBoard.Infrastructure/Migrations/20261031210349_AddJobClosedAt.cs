using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobBoard.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddJobClosedAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ClosedAt",
                table: "Job",
                type: "datetimeoffset",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClosedAt",
                table: "Job");
        }
    }
}
