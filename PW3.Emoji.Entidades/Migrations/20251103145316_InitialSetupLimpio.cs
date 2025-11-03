using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PW3.Emoji.Entidades.Migrations
{
    /// <inheritdoc />
    public partial class InitialSetupLimpio : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Emocion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nombre = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Descripcion = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Emocion__3214EC0709188CAD", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Rol",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nombre = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Rol__3214EC077CE478FD", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Usuario",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Email = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    HashPassword = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    Nombre = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    RolId = table.Column<int>(type: "INTEGER", nullable: false)
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
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UsuarioId = table.Column<int>(type: "INTEGER", nullable: false),
                    Ruta = table.Column<string>(type: "TEXT", maxLength: 400, nullable: false),
                    FechaSubida = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "(sysutcdatetime())"),
                    Ancho = table.Column<int>(type: "INTEGER", nullable: true),
                    Alto = table.Column<int>(type: "INTEGER", nullable: true)
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
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ImagenId = table.Column<int>(type: "INTEGER", nullable: false),
                    EmocionId = table.Column<int>(type: "INTEGER", nullable: false),
                    UsuarioId = table.Column<int>(type: "INTEGER", nullable: false),
                    Confianza = table.Column<double>(type: "REAL", nullable: false),
                    VectorJson = table.Column<string>(type: "TEXT", nullable: true),
                    FechaAnalisis = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "(sysutcdatetime())")
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
                name: "Emocion");

            migrationBuilder.DropTable(
                name: "Imagen");

            migrationBuilder.DropTable(
                name: "Usuario");

            migrationBuilder.DropTable(
                name: "Rol");
        }
    }
}
