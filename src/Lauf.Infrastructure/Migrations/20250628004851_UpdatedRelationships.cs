using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lauf.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FlowAssignmentProgress_FlowAssignments_FlowAssignmentId1",
                table: "FlowAssignmentProgress");

            migrationBuilder.DropForeignKey(
                name: "FK_FlowContents_Flows_FlowId1",
                table: "FlowContents");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Users_UserId1",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_UserId1",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_FlowContents_FlowId1",
                table: "FlowContents");

            migrationBuilder.DropIndex(
                name: "IX_FlowAssignmentProgress_FlowAssignmentId1",
                table: "FlowAssignmentProgress");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "FlowId1",
                table: "FlowContents");

            migrationBuilder.DropColumn(
                name: "FlowAssignmentId1",
                table: "FlowAssignmentProgress");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UserId1",
                table: "Notifications",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "FlowId1",
                table: "FlowContents",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "FlowAssignmentId1",
                table: "FlowAssignmentProgress",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId1",
                table: "Notifications",
                column: "UserId1");

            migrationBuilder.CreateIndex(
                name: "IX_FlowContents_FlowId1",
                table: "FlowContents",
                column: "FlowId1");

            migrationBuilder.CreateIndex(
                name: "IX_FlowAssignmentProgress_FlowAssignmentId1",
                table: "FlowAssignmentProgress",
                column: "FlowAssignmentId1");

            migrationBuilder.AddForeignKey(
                name: "FK_FlowAssignmentProgress_FlowAssignments_FlowAssignmentId1",
                table: "FlowAssignmentProgress",
                column: "FlowAssignmentId1",
                principalTable: "FlowAssignments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FlowContents_Flows_FlowId1",
                table: "FlowContents",
                column: "FlowId1",
                principalTable: "Flows",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Users_UserId1",
                table: "Notifications",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
