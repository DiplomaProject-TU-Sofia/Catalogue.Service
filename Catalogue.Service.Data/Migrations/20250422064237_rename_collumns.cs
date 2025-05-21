using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Catalogue.Service.Data.Migrations
{
    /// <inheritdoc />
    public partial class rename_collumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ServiceId",
                table: "Services",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "SaloonName",
                table: "Saloons",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "SaloonId",
                table: "Saloons",
                newName: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Services",
                newName: "ServiceId");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Saloons",
                newName: "SaloonName");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Saloons",
                newName: "SaloonId");
        }
    }
}
