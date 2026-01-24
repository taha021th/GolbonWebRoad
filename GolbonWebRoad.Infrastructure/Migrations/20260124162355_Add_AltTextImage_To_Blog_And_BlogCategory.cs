using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GolbonWebRoad.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Add_AltTextImage_To_Blog_And_BlogCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AltTextMainImageUrl",
                table: "Blogs",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AltTextImageUrl",
                table: "BlogCategories",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AltTextMainImageUrl",
                table: "Blogs");

            migrationBuilder.DropColumn(
                name: "AltTextImageUrl",
                table: "BlogCategories");
        }
    }
}
