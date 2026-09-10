using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Ruby.DAL.Migrations
{
    /// <inheritdoc />
    public partial class Add_Genres : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "genres",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_genres", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "track_genres",
                columns: table => new
                {
                    TrackId = table.Column<Guid>(type: "uuid", nullable: false),
                    GenreId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_track_genres", x => new { x.TrackId, x.GenreId });
                    table.ForeignKey(
                        name: "FK_track_genres_genres_GenreId",
                        column: x => x.GenreId,
                        principalTable: "genres",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_track_genres_tracks_TrackId",
                        column: x => x.TrackId,
                        principalTable: "tracks",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "genres",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { 1, "ClassicRock" },
                    { 2, "PunkRock" },
                    { 3, "Grunge" },
                    { 4, "AlternativeRock" },
                    { 5, "IndieRock" },
                    { 6, "HeavyMetal" },
                    { 7, "ThrashMetal" },
                    { 101, "Techno" },
                    { 102, "House" },
                    { 103, "Trance" },
                    { 104, "DrumAndBass" },
                    { 105, "Dubstep" },
                    { 106, "Synthwave" },
                    { 107, "Ambient" },
                    { 201, "OldSchoolHipHop" },
                    { 202, "Trap" },
                    { 203, "ContemporaryRnB" },
                    { 301, "Jazz" },
                    { 302, "Blues" },
                    { 303, "Funk" },
                    { 304, "Soul" },
                    { 401, "Classical" },
                    { 402, "Pop" },
                    { 403, "Country" },
                    { 404, "Reggae" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_track_genres_GenreId",
                table: "track_genres",
                column: "GenreId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "track_genres");

            migrationBuilder.DropTable(
                name: "genres");
        }
    }
}
