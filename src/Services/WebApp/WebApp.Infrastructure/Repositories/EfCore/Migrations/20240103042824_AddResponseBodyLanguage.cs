using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApp.Infrastructure.Repositories.EfCore.Migrations
{
    /// <inheritdoc />
    public partial class AddResponseBodyLanguage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ResponseText",
                table: "Routes",
                newName: "ResponseBodyLanguage");

            migrationBuilder.AddColumn<string>(
                name: "ResponseBody",
                table: "Routes",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResponseBody",
                table: "Routes");

            migrationBuilder.RenameColumn(
                name: "ResponseBodyLanguage",
                table: "Routes",
                newName: "ResponseText");
        }
    }
}
