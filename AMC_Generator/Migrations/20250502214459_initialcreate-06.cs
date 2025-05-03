using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMC_Generator.Migrations
{
    /// <inheritdoc />
    public partial class initialcreate06 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "TerminatedOn",
                table: "AMC",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TerminatedOn",
                table: "AMC");
        }
    }
}
