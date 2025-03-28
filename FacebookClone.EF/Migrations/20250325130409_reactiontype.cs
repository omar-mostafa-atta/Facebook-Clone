using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FacebookClone.EF.Migrations
{
    /// <inheritdoc />
    public partial class reactiontype : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "Reactions");

            migrationBuilder.AddColumn<int>(
                name: "ReactionType",
                table: "Reactions",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReactionType",
                table: "Reactions");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Reactions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
