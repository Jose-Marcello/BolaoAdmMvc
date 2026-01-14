using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevBol.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class RankingRodadas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RankingRodadas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RodadaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApostadorCampeonatoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Pontuacao = table.Column<int>(type: "int", nullable: false),
                    Posicao = table.Column<int>(type: "int", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RankingRodadas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RankingRodadas_ApostadoresCampeonatos_ApostadorCampeonatoId",
                        column: x => x.ApostadorCampeonatoId,
                        principalTable: "ApostadoresCampeonatos",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RankingRodadas_Rodadas_RodadaId",
                        column: x => x.RodadaId,
                        principalTable: "Rodadas",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_RankingRodadas_ApostadorCampeonatoId",
                table: "RankingRodadas",
                column: "ApostadorCampeonatoId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RankingRodadas_RodadaId",
                table: "RankingRodadas",
                column: "RodadaId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RankingRodadas");
        }
    }
}
