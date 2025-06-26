using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lauf.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveEmailAndPhoneFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FlowAssignments_Flows_FlowId1",
                table: "FlowAssignments");

            migrationBuilder.DropIndex(
                name: "IX_Users_Email",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_FlowAssignments_FlowId1",
                table: "FlowAssignments");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "FlowId1",
                table: "FlowAssignments");

            migrationBuilder.CreateIndex(
                name: "IX_Users_CreatedAt",
                table: "Users",
                column: "CreatedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_CreatedAt",
                table: "Users");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Users",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "FlowId1",
                table: "FlowAssignments",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_FlowAssignments_FlowId1",
                table: "FlowAssignments",
                column: "FlowId1");

            migrationBuilder.AddForeignKey(
                name: "FK_FlowAssignments_Flows_FlowId1",
                table: "FlowAssignments",
                column: "FlowId1",
                principalTable: "Flows",
                principalColumn: "Id");
        }
    }
}
