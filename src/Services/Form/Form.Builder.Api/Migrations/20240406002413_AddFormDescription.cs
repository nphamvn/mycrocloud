using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Form.Builder.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddFormDescription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Forms",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Forms");
        }
    }
}
