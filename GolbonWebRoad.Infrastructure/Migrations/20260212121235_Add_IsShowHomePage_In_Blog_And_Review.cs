using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GolbonWebRoad.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Add_IsShowHomePage_In_Blog_And_Review : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsShowHomePage",
                table: "Reviews",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsShowHomePage",
                table: "Blogs",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsShowHomePage",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "IsShowHomePage",
                table: "Blogs");
        }
    }
}
