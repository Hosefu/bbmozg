using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lauf.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveRedundantFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EstimatedDurationMinutes",
                table: "FlowSteps");

            migrationBuilder.DropColumn(
                name: "MaxAttempts",
                table: "FlowSettings");

            migrationBuilder.DropColumn(
                name: "MinPassingScorePercent",
                table: "FlowSettings");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "Flows");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "Flows");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EstimatedDurationMinutes",
                table: "FlowSteps",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MaxAttempts",
                table: "FlowSettings",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MinPassingScorePercent",
                table: "FlowSettings",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "Flows",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "Flows",
                type: "INTEGER",
                nullable: false,
                defaultValue: 1);
        }
    }
}
