using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApp.Infrastructure.Repositories.EfCore.Migrations
{
    /// <inheritdoc />
    public partial class AlterRoute : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResponseBodyDynamic",
                table: "Routes");

            migrationBuilder.DropColumn(
                name: "ResponseBodyInput",
                table: "Routes");

            migrationBuilder.RenameColumn(
                name: "ResponseBodyRenderEngine",
                table: "Routes",
                newName: "ResponseType");

            migrationBuilder.RenameColumn(
                name: "ResponseBodyPreScript",
                table: "Routes",
                newName: "FunctionHandler");

            migrationBuilder.AlterColumn<int>(
                name: "ResponseStatusCode",
                table: "Routes",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ResponseType",
                table: "Routes",
                newName: "ResponseBodyRenderEngine");

            migrationBuilder.RenameColumn(
                name: "FunctionHandler",
                table: "Routes",
                newName: "ResponseBodyPreScript");

            migrationBuilder.AlterColumn<int>(
                name: "ResponseStatusCode",
                table: "Routes",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ResponseBodyDynamic",
                table: "Routes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ResponseBodyInput",
                table: "Routes",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
