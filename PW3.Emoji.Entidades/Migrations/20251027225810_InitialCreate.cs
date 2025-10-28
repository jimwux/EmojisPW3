using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PW3.Emoji.Entidades.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Emocion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Emocion__3214EC0709188CAD", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Emoji",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CodigoUnicode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Alias = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Emoji__3214EC07EBBB0A7D", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Rol",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Rol__3214EC077CE478FD", x => x.Id);
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

            migrationBuilder.CreateTable(
                name: "Usuario",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    HashPassword = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    RolId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Usuario__3214EC07EBC80A05", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Usuario_Rol",
                        column: x => x.RolId,
                        principalTable: "Rol",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Imagen",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    Ruta = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: false),
                    FechaSubida = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(sysutcdatetime())"),
                    Ancho = table.Column<int>(type: "int", nullable: true),
                    Alto = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Imagen__3214EC0739CCA6A4", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Imagen_Usuario",
                        column: x => x.UsuarioId,
                        principalTable: "Usuario",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AnalisisResultado",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ImagenId = table.Column<int>(type: "int", nullable: false),
                    EmocionId = table.Column<int>(type: "int", nullable: false),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    Confianza = table.Column<double>(type: "float", nullable: false),
                    VectorJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaAnalisis = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(sysutcdatetime())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Analisis__3214EC07E3FD976B", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Analisis_Emocion",
                        column: x => x.EmocionId,
                        principalTable: "Emocion",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Analisis_Imagen",
                        column: x => x.ImagenId,
                        principalTable: "Imagen",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Analisis_Usuario",
                        column: x => x.UsuarioId,
                        principalTable: "Usuario",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Analisis_EmocionId",
                table: "AnalisisResultado",
                column: "EmocionId");

            migrationBuilder.CreateIndex(
                name: "IX_Analisis_ImagenId",
                table: "AnalisisResultado",
                column: "ImagenId");

            migrationBuilder.CreateIndex(
                name: "IX_Analisis_UsuarioId",
                table: "AnalisisResultado",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "UQ_Emocion_Nombre",
                table: "Emocion",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Imagen_UsuarioId",
                table: "Imagen",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_MapeoEmocionEmoji_EmojiId",
                table: "MapeoEmocionEmoji",
                column: "EmojiId");

            migrationBuilder.CreateIndex(
                name: "UQ_Mapeo_EmocionId",
                table: "MapeoEmocionEmoji",
                column: "EmocionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuario_RolId",
                table: "Usuario",
                column: "RolId");

            migrationBuilder.CreateIndex(
                name: "UQ_Usuario_Email",
                table: "Usuario",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnalisisResultado");

            migrationBuilder.DropTable(
                name: "MapeoEmocionEmoji");

            migrationBuilder.DropTable(
                name: "Imagen");

            migrationBuilder.DropTable(
                name: "Emocion");

            migrationBuilder.DropTable(
                name: "Emoji");

            migrationBuilder.DropTable(
                name: "Usuario");

            migrationBuilder.DropTable(
                name: "Rol");
        }
    }
}
