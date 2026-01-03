using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SolutionOrdersReact.Server.Migrations
{
    /// <inheritdoc />
    public partial class OrderWithOrderItemsAndWorkersMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Workers",
                columns: table => new
                {
                    IdWorker = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PostalCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Workers", x => x.IdWorker);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    IdOrder = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DataOrder = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    DeliveryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IdClient = table.Column<int>(type: "int", nullable: true),
                    IdWorker = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.IdOrder);
                    table.ForeignKey(
                        name: "FK_Orders_Clients_IdClient",
                        column: x => x.IdClient,
                        principalTable: "Clients",
                        principalColumn: "IdClient",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Orders_Workers_IdWorker",
                        column: x => x.IdWorker,
                        principalTable: "Workers",
                        principalColumn: "IdWorker",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "OrderItems",
                columns: table => new
                {
                    IdOrderItem = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdOrder = table.Column<int>(type: "int", nullable: false),
                    IdItem = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItems", x => x.IdOrderItem);
                    table.ForeignKey(
                        name: "FK_OrderItems_Items_IdItem",
                        column: x => x.IdItem,
                        principalTable: "Items",
                        principalColumn: "IdItem",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderItems_Orders_IdOrder",
                        column: x => x.IdOrder,
                        principalTable: "Orders",
                        principalColumn: "IdOrder",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Clients",
                keyColumn: "IdClient",
                keyValue: 1,
                columns: new[] { "Address", "PostalCode" },
                values: new object[] { "ul. Główna 1", "00-001" });

            migrationBuilder.UpdateData(
                table: "Clients",
                keyColumn: "IdClient",
                keyValue: 2,
                columns: new[] { "Address", "PostalCode" },
                values: new object[] { "ul. Rynek 5", "30-001" });

            migrationBuilder.InsertData(
                table: "Workers",
                columns: new[] { "IdWorker", "Address", "City", "Email", "FirstName", "IsActive", "LastName", "Phone", "PostalCode" },
                values: new object[,]
                {
                    { 1, "ul. Handlowa 10", "Warszawa", "piotr.wisniewski@firma.pl", "Piotr", true, "Wiśniewski", "111222333", "00-100" },
                    { 2, "ul. Biurowa 15", "Kraków", "maria.zielinska@firma.pl", "Maria", true, "Zielińska", "444555666", "30-100" }
                });

            migrationBuilder.InsertData(
                table: "Orders",
                columns: new[] { "IdOrder", "DataOrder", "DeliveryDate", "IdClient", "IdWorker", "IsActive", "Notes" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 1, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 1, true, "Pierwsze zamówienie testowe" },
                    { 2, new DateTime(2024, 2, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 2, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 2, true, "Drugie zamówienie testowe" }
                });

            migrationBuilder.InsertData(
                table: "OrderItems",
                columns: new[] { "IdOrderItem", "IdItem", "IdOrder", "IsActive", "Quantity", "UnitPrice" },
                values: new object[,]
                {
                    { 1, 1, 1, true, 2m, 3500m },
                    { 2, 2, 1, true, 3m, 800m },
                    { 3, 3, 2, true, 5m, 150m },
                    { 4, 1, 2, true, 1m, 3500m }
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_IdItem",
                table: "OrderItems",
                column: "IdItem");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_IdOrder",
                table: "OrderItems",
                column: "IdOrder");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_IdClient",
                table: "Orders",
                column: "IdClient");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_IdWorker",
                table: "Orders",
                column: "IdWorker");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderItems");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Workers");

            migrationBuilder.UpdateData(
                table: "Clients",
                keyColumn: "IdClient",
                keyValue: 1,
                columns: new[] { "Address", "PostalCode" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Clients",
                keyColumn: "IdClient",
                keyValue: 2,
                columns: new[] { "Address", "PostalCode" },
                values: new object[] { null, null });
        }
    }
}
