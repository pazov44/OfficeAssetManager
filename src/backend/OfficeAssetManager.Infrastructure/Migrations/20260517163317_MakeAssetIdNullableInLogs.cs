using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OfficeAssetManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MakeAssetIdNullableInLogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssetLogs_Assets_AssetId",
                table: "AssetLogs");

            migrationBuilder.AlterColumn<int>(
                name: "AssetId",
                table: "AssetLogs",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetLogs_Assets_AssetId",
                table: "AssetLogs",
                column: "AssetId",
                principalTable: "Assets",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssetLogs_Assets_AssetId",
                table: "AssetLogs");

            migrationBuilder.AlterColumn<int>(
                name: "AssetId",
                table: "AssetLogs",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetLogs_Assets_AssetId",
                table: "AssetLogs",
                column: "AssetId",
                principalTable: "Assets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
