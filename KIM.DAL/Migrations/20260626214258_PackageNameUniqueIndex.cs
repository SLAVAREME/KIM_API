using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KIM.DAL.Migrations
{
    /// <inheritdoc />
    public partial class PackageNameUniqueIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_QuestionPackages_Name",
                table: "QuestionPackages",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_QuestionPackages_Name",
                table: "QuestionPackages");
        }
    }
}
