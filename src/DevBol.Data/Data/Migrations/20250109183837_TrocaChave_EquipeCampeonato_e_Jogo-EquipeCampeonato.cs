using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevBol.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class TrocaChave_EquipeCampeonato_e_JogoEquipeCampeonato : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Jogos_Equipes_EquipeCasaId",
                table: "Jogos");

            migrationBuilder.DropForeignKey(
                name: "FK_Jogos_Equipes_EquipeVisitanteId",
                table: "Jogos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EquipesCampeonatos",
                table: "EquipesCampeonatos");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EquipesCampeonatos",
                table: "EquipesCampeonatos",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_EquipesCampeonatos_EquipeId",
                table: "EquipesCampeonatos",
                column: "EquipeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Jogos_EquipesCampeonatos_EquipeCasaId",
                table: "Jogos",
                column: "EquipeCasaId",
                principalTable: "EquipesCampeonatos",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Jogos_EquipesCampeonatos_EquipeVisitanteId",
                table: "Jogos",
                column: "EquipeVisitanteId",
                principalTable: "EquipesCampeonatos",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Jogos_EquipesCampeonatos_EquipeCasaId",
                table: "Jogos");

            migrationBuilder.DropForeignKey(
                name: "FK_Jogos_EquipesCampeonatos_EquipeVisitanteId",
                table: "Jogos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EquipesCampeonatos",
                table: "EquipesCampeonatos");

            migrationBuilder.DropIndex(
                name: "IX_EquipesCampeonatos_EquipeId",
                table: "EquipesCampeonatos");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EquipesCampeonatos",
                table: "EquipesCampeonatos",
                columns: new[] { "EquipeId", "CampeonatoId" });

            migrationBuilder.AddForeignKey(
                name: "FK_Jogos_Equipes_EquipeCasaId",
                table: "Jogos",
                column: "EquipeCasaId",
                principalTable: "Equipes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Jogos_Equipes_EquipeVisitanteId",
                table: "Jogos",
                column: "EquipeVisitanteId",
                principalTable: "Equipes",
                principalColumn: "Id");
        }
    }
}
