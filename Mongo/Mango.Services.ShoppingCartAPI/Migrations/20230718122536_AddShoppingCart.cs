﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mango.Services.ShoppingCartAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddShoppingCart : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CartHeaders",
                columns: table => new
                {
                    CardHeaderId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CouponCode = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartHeaders", x => x.CardHeaderId);
                });

            migrationBuilder.CreateTable(
                name: "CartDetails",
                columns: table => new
                {
                    CardDetailsId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CardHeaderId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Count = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartDetails", x => x.CardDetailsId);
                    table.ForeignKey(
                        name: "FK_CartDetails_CartHeaders_CardHeaderId",
                        column: x => x.CardHeaderId,
                        principalTable: "CartHeaders",
                        principalColumn: "CardHeaderId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CartDetails_CardHeaderId",
                table: "CartDetails",
                column: "CardHeaderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CartDetails");

            migrationBuilder.DropTable(
                name: "CartHeaders");
        }
    }
}
