using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GolbonWebRoad.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Add_Slog_Unic_In_Brand : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Slog",
                table: "Brands",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Brands_Slog",
                table: "Brands",
                column: "Slog",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Brands_Slog",
                table: "Brands");

            migrationBuilder.DropColumn(
                name: "Slog",
                table: "Brands");
        }
    }
}
