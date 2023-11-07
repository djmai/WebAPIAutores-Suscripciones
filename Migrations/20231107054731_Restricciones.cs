using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPIAutores.Migrations
{
    /// <inheritdoc />
    public partial class Restricciones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RestriccionesDominio",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LlaveId = table.Column<int>(type: "int", nullable: false),
                    Dominio = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RestriccionesDominio", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RestriccionesDominio_LlavesAPI_LlaveId",
                        column: x => x.LlaveId,
                        principalTable: "LlavesAPI",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RestricionesIP",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LlaveId = table.Column<int>(type: "int", nullable: false),
                    IP = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RestricionesIP", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RestricionesIP_LlavesAPI_LlaveId",
                        column: x => x.LlaveId,
                        principalTable: "LlavesAPI",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RestriccionesDominio_LlaveId",
                table: "RestriccionesDominio",
                column: "LlaveId");

            migrationBuilder.CreateIndex(
                name: "IX_RestricionesIP_LlaveId",
                table: "RestricionesIP",
                column: "LlaveId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RestriccionesDominio");

            migrationBuilder.DropTable(
                name: "RestricionesIP");
        }
    }
}
