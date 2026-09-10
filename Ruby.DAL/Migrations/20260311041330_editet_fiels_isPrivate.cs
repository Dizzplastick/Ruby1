using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ruby.DAL.Migrations
{
    /// <inheritdoc />
    public partial class editet_fiels_isPrivate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsPrivate",
                table: "playlists",
                newName: "is_private");

            migrationBuilder.AlterColumn<bool>(
                name: "is_private",
                table: "playlists",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "is_private",
                table: "playlists",
                newName: "IsPrivate");

            migrationBuilder.AlterColumn<bool>(
                name: "IsPrivate",
                table: "playlists",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);
        }
    }
}
