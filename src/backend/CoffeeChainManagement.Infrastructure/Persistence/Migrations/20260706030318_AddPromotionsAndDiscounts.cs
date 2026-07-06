using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoffeeChainManagement.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPromotionsAndDiscounts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "DiscountAmount",
                table: "sale_orders",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "PromotionCode",
                table: "sale_orders",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "DiscountPercent",
                table: "promotions",
                type: "numeric(5,2)",
                precision: 5,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(5,2)",
                oldPrecision: 5,
                oldScale: 2);

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "promotions",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "DiscountAmount",
                table: "promotions",
                type: "numeric",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiscountAmount",
                table: "sale_orders");

            migrationBuilder.DropColumn(
                name: "PromotionCode",
                table: "sale_orders");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "promotions");

            migrationBuilder.DropColumn(
                name: "DiscountAmount",
                table: "promotions");

            migrationBuilder.AlterColumn<decimal>(
                name: "DiscountPercent",
                table: "promotions",
                type: "numeric(5,2)",
                precision: 5,
                scale: 2,
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "numeric(5,2)",
                oldPrecision: 5,
                oldScale: 2,
                oldNullable: true);
        }
    }
}
