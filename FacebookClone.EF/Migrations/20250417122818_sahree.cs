using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FacebookClone.EF.Migrations
{
    /// <inheritdoc />
    public partial class sahree : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TotalShares",
                table: "Post",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalShares",
                table: "Post");
        }
    }
}
