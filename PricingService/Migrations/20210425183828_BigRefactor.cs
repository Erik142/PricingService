using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PricingService.Migrations
{
    public partial class BigRefactor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BasePrices");

            migrationBuilder.DropColumn(
                name: "ServiceName",
                table: "FreeDays");

            migrationBuilder.DropColumn(
                name: "ConsumerId",
                table: "Discounts");

            migrationBuilder.RenameColumn(
                name: "ConsumerId",
                table: "FreeDays",
                newName: "CustomerId");

            migrationBuilder.AlterColumn<int>(
                name: "FreeDays",
                table: "FreeDays",
                type: "int",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(20,0)");

            migrationBuilder.AlterColumn<string>(
                name: "ServiceName",
                table: "Discounts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FreeDaysId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Customers_FreeDays_FreeDaysId",
                        column: x => x.FreeDaysId,
                        principalTable: "FreeDays",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ServiceDefinitions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServiceName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<double>(type: "float", nullable: false),
                    Discriminator = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DiscountId = table.Column<int>(type: "int", nullable: true),
                    CustomerModelId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceDefinitions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceDefinitions_Customers_CustomerModelId",
                        column: x => x.CustomerModelId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ServiceDefinitions_Discounts_DiscountId",
                        column: x => x.DiscountId,
                        principalTable: "Discounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Days",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Day = table.Column<int>(type: "int", nullable: false),
                    ServiceDefinitionModelId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Days", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Days_ServiceDefinitions_ServiceDefinitionModelId",
                        column: x => x.ServiceDefinitionModelId,
                        principalTable: "ServiceDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Customers_FreeDaysId",
                table: "Customers",
                column: "FreeDaysId");

            migrationBuilder.CreateIndex(
                name: "IX_Days_ServiceDefinitionModelId",
                table: "Days",
                column: "ServiceDefinitionModelId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceDefinitions_CustomerModelId",
                table: "ServiceDefinitions",
                column: "CustomerModelId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceDefinitions_DiscountId",
                table: "ServiceDefinitions",
                column: "DiscountId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Days");

            migrationBuilder.DropTable(
                name: "ServiceDefinitions");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.RenameColumn(
                name: "CustomerId",
                table: "FreeDays",
                newName: "ConsumerId");

            migrationBuilder.AlterColumn<decimal>(
                name: "FreeDays",
                table: "FreeDays",
                type: "decimal(20,0)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "ServiceName",
                table: "FreeDays",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ServiceName",
                table: "Discounts",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "ConsumerId",
                table: "Discounts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "BasePrices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BasePrice = table.Column<double>(type: "float", nullable: false),
                    ConsumerId = table.Column<int>(type: "int", nullable: false),
                    ServiceName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BasePrices", x => x.Id);
                });
        }
    }
}
