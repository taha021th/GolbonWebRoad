using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace GolbonWebRoad.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFaqCategoryAndLink : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "Faqs");

            migrationBuilder.AddColumn<int>(
                name: "FaqCategoryId",
                table: "Faqs",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "FaqCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Slog = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FaqCategories", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Faqs_FaqCategoryId",
                table: "Faqs",
                column: "FaqCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_FaqCategories_Slog",
                table: "FaqCategories",
                column: "Slog",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Faqs_FaqCategories_FaqCategoryId",
                table: "Faqs",
                column: "FaqCategoryId",
                principalTable: "FaqCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Faqs_FaqCategories_FaqCategoryId",
                table: "Faqs");

            migrationBuilder.DropTable(
                name: "FaqCategories");

            migrationBuilder.DropIndex(
                name: "IX_Faqs_FaqCategoryId",
                table: "Faqs");

            migrationBuilder.DropColumn(
                name: "FaqCategoryId",
                table: "Faqs");

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "Faqs",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);
        }
    }
}
