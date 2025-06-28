using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lauf.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixedRelationshipsDuplication : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FlowContents_Users_CreatedByUserId",
                table: "FlowContents");

            migrationBuilder.DropIndex(
                name: "IX_FlowContents_CreatedByUserId",
                table: "FlowContents");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "FlowContents");

            migrationBuilder.CreateIndex(
                name: "IX_FlowContents_CreatedBy",
                table: "FlowContents",
                column: "CreatedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_FlowContents_Users_CreatedBy",
                table: "FlowContents",
                column: "CreatedBy",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FlowContents_Users_CreatedBy",
                table: "FlowContents");

            migrationBuilder.DropIndex(
                name: "IX_FlowContents_CreatedBy",
                table: "FlowContents");

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "FlowContents",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_FlowContents_CreatedByUserId",
                table: "FlowContents",
                column: "CreatedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_FlowContents_Users_CreatedByUserId",
                table: "FlowContents",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
