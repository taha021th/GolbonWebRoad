using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GolbonWebRoad.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Add_MainImageAltUrl_To_Product : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Gtin",
                table: "ProductVariants",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Mpn",
                table: "ProductVariants",
                type: "text",
                nullable: true);



            migrationBuilder.AddColumn<string>(
                name: "MainImageAltText",
                table: "Products",
                type: "text",
                nullable: true);


        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Gtin",
                table: "ProductVariants");

            migrationBuilder.DropColumn(
                name: "Mpn",
                table: "ProductVariants");


            migrationBuilder.DropColumn(
                name: "MainImageAltText",
                table: "Products");

        }
    }
}
