using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class RemoveVehicleAddOrderTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InboundDelivery_Vehicle_VehicleId",
                schema: "Warehouse",
                table: "InboundDelivery");

            migrationBuilder.DropForeignKey(
                name: "FK_OutboundDelivery_Vehicle_VehicleId",
                schema: "Warehouse",
                table: "OutboundDelivery");

            migrationBuilder.DropTable(
                name: "Vehicle",
                schema: "Logistics");

            migrationBuilder.DropIndex(
                name: "IX_OutboundDelivery_VehicleId",
                schema: "Warehouse",
                table: "OutboundDelivery");

            migrationBuilder.DropIndex(
                name: "IX_InboundDelivery_VehicleId",
                schema: "Warehouse",
                table: "InboundDelivery");

            migrationBuilder.DropColumn(
                name: "VehicleId",
                schema: "Warehouse",
                table: "OutboundDelivery");

            migrationBuilder.DropColumn(
                name: "VehicleId",
                schema: "Warehouse",
                table: "InboundDelivery");

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdentityUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    StoreId = table.Column<int>(type: "int", nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Employees_AspNetUsers_IdentityUserId",
                        column: x => x.IdentityUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Order",
                schema: "Warehouse",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Order", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Order_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderItem",
                schema: "Warehouse",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<long>(type: "bigint", nullable: false),
                    WarehouseItemId = table.Column<long>(type: "bigint", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderItem_Item_WarehouseItemId",
                        column: x => x.WarehouseItemId,
                        principalSchema: "Warehouse",
                        principalTable: "Item",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderItem_Order_OrderId",
                        column: x => x.OrderId,
                        principalSchema: "Warehouse",
                        principalTable: "Order",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Employees_IdentityUserId",
                table: "Employees",
                column: "IdentityUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Order_UserId",
                schema: "Warehouse",
                table: "Order",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItem_OrderId",
                schema: "Warehouse",
                table: "OrderItem",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItem_WarehouseItemId",
                schema: "Warehouse",
                table: "OrderItem",
                column: "WarehouseItemId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "OrderItem",
                schema: "Warehouse");

            migrationBuilder.DropTable(
                name: "Order",
                schema: "Warehouse");

            migrationBuilder.EnsureSchema(
                name: "Logistics");

            migrationBuilder.AddColumn<long>(
                name: "VehicleId",
                schema: "Warehouse",
                table: "OutboundDelivery",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "VehicleId",
                schema: "Warehouse",
                table: "InboundDelivery",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Vehicle",
                schema: "Logistics",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VendorId = table.Column<long>(type: "bigint", nullable: false),
                    Brand = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DriverName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    MaxVolumeCapacityCbm = table.Column<decimal>(type: "decimal(12,4)", nullable: false),
                    MaxWeightCapacityKg = table.Column<decimal>(type: "decimal(12,3)", nullable: false),
                    Model = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PlateNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vehicle", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Vehicle_Vendor_VendorId",
                        column: x => x.VendorId,
                        principalSchema: "Warehouse",
                        principalTable: "Vendor",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OutboundDelivery_VehicleId",
                schema: "Warehouse",
                table: "OutboundDelivery",
                column: "VehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_InboundDelivery_VehicleId",
                schema: "Warehouse",
                table: "InboundDelivery",
                column: "VehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicle_VendorId",
                schema: "Logistics",
                table: "Vehicle",
                column: "VendorId");

            migrationBuilder.AddForeignKey(
                name: "FK_InboundDelivery_Vehicle_VehicleId",
                schema: "Warehouse",
                table: "InboundDelivery",
                column: "VehicleId",
                principalSchema: "Logistics",
                principalTable: "Vehicle",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OutboundDelivery_Vehicle_VehicleId",
                schema: "Warehouse",
                table: "OutboundDelivery",
                column: "VehicleId",
                principalSchema: "Logistics",
                principalTable: "Vehicle",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
