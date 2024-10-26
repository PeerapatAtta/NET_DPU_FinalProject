using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddCatalogandAdmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CatalogId",
                table: "Products",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "Catalogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Catalogs", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Description", "Name", "NormalizedName" },
                values: new object[] { new Guid("82b98abf-4f3e-4e4a-b5d8-bff84b3e48d2"), null, "Admin role with full permissions", "Admin", "ADMIN" });

            migrationBuilder.CreateIndex(
                name: "IX_Products_CatalogId",
                table: "Products",
                column: "CatalogId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Catalogs_CatalogId",
                table: "Products",
                column: "CatalogId",
                principalTable: "Catalogs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Catalogs_CatalogId",
                table: "Products");

            migrationBuilder.DropTable(
                name: "Catalogs");

            migrationBuilder.DropIndex(
                name: "IX_Products_CatalogId",
                table: "Products");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("82b98abf-4f3e-4e4a-b5d8-bff84b3e48d2"));

            migrationBuilder.DropColumn(
                name: "CatalogId",
                table: "Products");
        }
    }
}
