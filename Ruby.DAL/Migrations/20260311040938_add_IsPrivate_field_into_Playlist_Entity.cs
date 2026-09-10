using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ruby.DAL.Migrations
{
    /// <inheritdoc />
    public partial class add_IsPrivate_field_into_Playlist_Entity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPrivate",
                table: "playlists",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPrivate",
                table: "playlists");
        }
    }
}
