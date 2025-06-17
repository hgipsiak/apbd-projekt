using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace projekt.Migrations
{
    /// <inheritdoc />
    public partial class Update2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Discount",
                columns: table => new
                {
                    DiscountId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Value = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    FromDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    ToDate = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Discount", x => x.DiscountId);
                });

            migrationBuilder.CreateTable(
                name: "OperatingSystem",
                columns: table => new
                {
                    SoftwareId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Version = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OperatingSystem", x => x.SoftwareId);
                });

            migrationBuilder.CreateTable(
                name: "DiscountOperatingSystem",
                columns: table => new
                {
                    SoftwareId = table.Column<int>(type: "int", nullable: false),
                    DiscountId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscountOperatingSystem", x => new { x.DiscountId, x.SoftwareId });
                    table.ForeignKey(
                        name: "FK_DiscountOperatingSystem_Discount_DiscountId",
                        column: x => x.DiscountId,
                        principalTable: "Discount",
                        principalColumn: "DiscountId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DiscountOperatingSystem_OperatingSystem_SoftwareId",
                        column: x => x.SoftwareId,
                        principalTable: "OperatingSystem",
                        principalColumn: "SoftwareId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DiscountOperatingSystem_SoftwareId",
                table: "DiscountOperatingSystem",
                column: "SoftwareId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DiscountOperatingSystem");

            migrationBuilder.DropTable(
                name: "Discount");

            migrationBuilder.DropTable(
                name: "OperatingSystem");
        }
    }
}
