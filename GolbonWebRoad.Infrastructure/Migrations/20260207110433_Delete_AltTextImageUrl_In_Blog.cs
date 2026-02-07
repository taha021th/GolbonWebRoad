using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GolbonWebRoad.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Delete_AltTextImageUrl_In_Blog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AltTextMainImageUrl",
                table: "Blogs");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AltTextMainImageUrl",
                table: "Blogs",
                type: "text",
                nullable: true);
        }
    }
}
