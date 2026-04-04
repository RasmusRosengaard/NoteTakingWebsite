using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace webapi.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Role",
                table: "Canvases");

            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "UserCanvases",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Role",
                table: "UserCanvases");

            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "Canvases",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
