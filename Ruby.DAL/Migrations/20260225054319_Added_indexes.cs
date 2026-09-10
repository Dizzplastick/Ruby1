using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ruby.DAL.Migrations
{
    /// <inheritdoc />
    public partial class Added_indexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "email",
                table: "users",
                newName: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_users_Email",
                table: "users",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_users_username",
                table: "users",
                column: "username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tracks_created_at",
                table: "tracks",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "IX_tracks_title",
                table: "tracks",
                column: "title");

            migrationBuilder.CreateIndex(
                name: "IX_playlists_title",
                table: "playlists",
                column: "title");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_users_Email",
                table: "users");

            migrationBuilder.DropIndex(
                name: "IX_users_username",
                table: "users");

            migrationBuilder.DropIndex(
                name: "IX_tracks_created_at",
                table: "tracks");

            migrationBuilder.DropIndex(
                name: "IX_tracks_title",
                table: "tracks");

            migrationBuilder.DropIndex(
                name: "IX_playlists_title",
                table: "playlists");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "users",
                newName: "email");
        }
    }
}
