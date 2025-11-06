using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PW3.Emoji.Entidades.Migrations
{
    /// <inheritdoc />
    public partial class Actualizacion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MapeoEmocionEmoji");

            migrationBuilder.DropTable(
                name: "Emoji");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Emoji",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Alias = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CodigoUnicode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Emoji__3214EC07EBBB0A7D", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MapeoEmocionEmoji",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmocionId = table.Column<int>(type: "int", nullable: false),
                    EmojiId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__MapeoEmo__3214EC075C79F2E8", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Mapeo_Emocion",
                        column: x => x.EmocionId,
                        principalTable: "Emocion",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Mapeo_Emoji",
                        column: x => x.EmojiId,
                        principalTable: "Emoji",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_MapeoEmocionEmoji_EmojiId",
                table: "MapeoEmocionEmoji",
                column: "EmojiId");

            migrationBuilder.CreateIndex(
                name: "UQ_Mapeo_EmocionId",
                table: "MapeoEmocionEmoji",
                column: "EmocionId",
                unique: true);
        }
    }
}
