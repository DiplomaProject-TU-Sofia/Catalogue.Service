using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Catalogue.Service.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddImageNamesToSaloonAndServices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageName",
                table: "Services",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageName",
                table: "Saloons",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageName",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "ImageName",
                table: "Saloons");
        }
    }
}
