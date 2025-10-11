using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GolbonWebRoad.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Add_Weight_Length_Width_Height_To_Product : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Height",
                table: "ProductVariants",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Length",
                table: "ProductVariants",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "Weight",
                table: "ProductVariants",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "Width",
                table: "ProductVariants",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Height",
                table: "ProductVariants");

            migrationBuilder.DropColumn(
                name: "Length",
                table: "ProductVariants");

            migrationBuilder.DropColumn(
                name: "Weight",
                table: "ProductVariants");

            migrationBuilder.DropColumn(
                name: "Width",
                table: "ProductVariants");
        }
    }
}
