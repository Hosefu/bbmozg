using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lauf.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveQuizComponentFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AllowMultipleAttempts",
                table: "QuizComponents");

            migrationBuilder.DropColumn(
                name: "ShuffleOptions",
                table: "QuizComponents");

            migrationBuilder.DropColumn(
                name: "ShuffleQuestions",
                table: "QuizComponents");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AllowMultipleAttempts",
                table: "QuizComponents",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ShuffleOptions",
                table: "QuizComponents",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ShuffleQuestions",
                table: "QuizComponents",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }
    }
}
