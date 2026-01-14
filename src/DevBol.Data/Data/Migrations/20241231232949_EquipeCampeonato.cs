using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevBol.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class EquipeCampeonato : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EquipesCampeonatos",
                columns: table => new
                {
                    CampeonatoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EquipeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipesCampeonatos", x => new { x.EquipeId, x.CampeonatoId });
                    table.ForeignKey(
                        name: "FK_EquipesCampeonatos_Campeonatos_CampeonatoId",
                        column: x => x.CampeonatoId,
                        principalTable: "Campeonatos",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EquipesCampeonatos_Equipes_EquipeId",
                        column: x => x.EquipeId,
                        principalTable: "Equipes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_EquipesCampeonatos_CampeonatoId",
                table: "EquipesCampeonatos",
                column: "CampeonatoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EquipesCampeonatos");
        }
    }
}
