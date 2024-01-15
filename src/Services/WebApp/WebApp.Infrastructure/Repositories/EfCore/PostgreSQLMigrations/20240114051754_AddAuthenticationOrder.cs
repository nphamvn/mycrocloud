using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApp.Infrastructure.Repositories.EfCore.Migrations
{
    /// <inheritdoc />
    public partial class AddAuthenticationOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UseAuthentication",
                table: "Apps");

            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "AuthenticationSchemes",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Order",
                table: "AuthenticationSchemes");

            migrationBuilder.AddColumn<bool>(
                name: "UseAuthentication",
                table: "Apps",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
