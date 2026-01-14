using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevBol.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class ApostadorCampeonato : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApostadoresCampeonatos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CampeonatoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApostadorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApostadoresCampeonatos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApostadoresCampeonatos_Apostadores_ApostadorId",
                        column: x => x.ApostadorId,
                        principalTable: "Apostadores",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ApostadoresCampeonatos_Campeonatos_CampeonatoId",
                        column: x => x.CampeonatoId,
                        principalTable: "Campeonatos",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApostadoresCampeonatos_ApostadorId",
                table: "ApostadoresCampeonatos",
                column: "ApostadorId");

            migrationBuilder.CreateIndex(
                name: "IX_ApostadoresCampeonatos_CampeonatoId",
                table: "ApostadoresCampeonatos",
                column: "CampeonatoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApostadoresCampeonatos");
        }
    }
}
