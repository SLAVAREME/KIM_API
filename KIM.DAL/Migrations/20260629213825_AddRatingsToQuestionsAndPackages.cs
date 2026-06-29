using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KIM.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddRatingsToQuestionsAndPackages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Rating",
                table: "Questions",
                type: "float(3)",
                precision: 3,
                scale: 2,
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "RatingVotesCount",
                table: "Questions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "Rating",
                table: "QuestionPackages",
                type: "float(3)",
                precision: 3,
                scale: 2,
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "RatingVotesCount",
                table: "QuestionPackages",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Rating",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "RatingVotesCount",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "Rating",
                table: "QuestionPackages");

            migrationBuilder.DropColumn(
                name: "RatingVotesCount",
                table: "QuestionPackages");
        }
    }
}
