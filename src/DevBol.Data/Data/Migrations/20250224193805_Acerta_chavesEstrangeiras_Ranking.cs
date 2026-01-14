using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevBol.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class Acerta_chavesEstrangeiras_Ranking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RankingRodadas_ApostadorCampeonatoId",
                table: "RankingRodadas");

            migrationBuilder.DropIndex(
                name: "IX_RankingRodadas_RodadaId",
                table: "RankingRodadas");

            migrationBuilder.CreateIndex(
                name: "IX_RankingRodadas_ApostadorCampeonatoId",
                table: "RankingRodadas",
                column: "ApostadorCampeonatoId");

            migrationBuilder.CreateIndex(
                name: "IX_RankingRodadas_RodadaId",
                table: "RankingRodadas",
                column: "RodadaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RankingRodadas_ApostadorCampeonatoId",
                table: "RankingRodadas");

            migrationBuilder.DropIndex(
                name: "IX_RankingRodadas_RodadaId",
                table: "RankingRodadas");

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
    }
}
