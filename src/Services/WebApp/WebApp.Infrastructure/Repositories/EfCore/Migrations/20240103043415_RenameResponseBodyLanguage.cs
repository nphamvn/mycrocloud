using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApp.Infrastructure.Repositories.EfCore.Migrations
{
    /// <inheritdoc />
    public partial class RenameResponseBodyLanguage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResponseBodyLanguage",
                table: "Routes");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ResponseBodyLanguage",
                table: "Routes",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
