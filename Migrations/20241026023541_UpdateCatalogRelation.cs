using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCatalogRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Catalogs_CatalogId",
                table: "Products");

            migrationBuilder.AlterColumn<Guid>(
                name: "CatalogId",
                table: "Products",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "TEXT");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Catalogs_CatalogId",
                table: "Products",
                column: "CatalogId",
                principalTable: "Catalogs",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Catalogs_CatalogId",
                table: "Products");

            migrationBuilder.AlterColumn<Guid>(
                name: "CatalogId",
                table: "Products",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Catalogs_CatalogId",
                table: "Products",
                column: "CatalogId",
                principalTable: "Catalogs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
