using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoffeeChainManagement.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPromotionTargets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "BranchId",
                table: "promotions",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerPhone",
                table: "promotions",
                type: "character varying(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerSegment",
                table: "promotions",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_promotions_BranchId",
                table: "promotions",
                column: "BranchId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_promotions_BranchId",
                table: "promotions");

            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "promotions");

            migrationBuilder.DropColumn(
                name: "CustomerPhone",
                table: "promotions");

            migrationBuilder.DropColumn(
                name: "CustomerSegment",
                table: "promotions");
        }
    }
}
