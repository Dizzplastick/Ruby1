using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ruby.DAL.Migrations
{
    /// <inheritdoc />
    public partial class edit_names_of_columns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_track_genres_genres_GenreId",
                table: "track_genres");

            migrationBuilder.DropForeignKey(
                name: "FK_track_genres_tracks_TrackId",
                table: "track_genres");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "users",
                newName: "email");

            migrationBuilder.RenameIndex(
                name: "IX_users_Email",
                table: "users",
                newName: "IX_users_email");

            migrationBuilder.RenameColumn(
                name: "GenreId",
                table: "track_genres",
                newName: "genre_id");

            migrationBuilder.RenameColumn(
                name: "TrackId",
                table: "track_genres",
                newName: "track_id");

            migrationBuilder.RenameIndex(
                name: "IX_track_genres_GenreId",
                table: "track_genres",
                newName: "IX_track_genres_genre_id");

            migrationBuilder.AddForeignKey(
                name: "FK_track_genres_genres_genre_id",
                table: "track_genres",
                column: "genre_id",
                principalTable: "genres",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_track_genres_tracks_track_id",
                table: "track_genres",
                column: "track_id",
                principalTable: "tracks",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_track_genres_genres_genre_id",
                table: "track_genres");

            migrationBuilder.DropForeignKey(
                name: "FK_track_genres_tracks_track_id",
                table: "track_genres");

            migrationBuilder.RenameColumn(
                name: "email",
                table: "users",
                newName: "Email");

            migrationBuilder.RenameIndex(
                name: "IX_users_email",
                table: "users",
                newName: "IX_users_Email");

            migrationBuilder.RenameColumn(
                name: "genre_id",
                table: "track_genres",
                newName: "GenreId");

            migrationBuilder.RenameColumn(
                name: "track_id",
                table: "track_genres",
                newName: "TrackId");

            migrationBuilder.RenameIndex(
                name: "IX_track_genres_genre_id",
                table: "track_genres",
                newName: "IX_track_genres_GenreId");

            migrationBuilder.AddForeignKey(
                name: "FK_track_genres_genres_GenreId",
                table: "track_genres",
                column: "GenreId",
                principalTable: "genres",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_track_genres_tracks_TrackId",
                table: "track_genres",
                column: "TrackId",
                principalTable: "tracks",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
