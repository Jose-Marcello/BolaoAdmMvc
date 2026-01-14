using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevBol.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class Jogo_Rodada : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "EquipeCasaId",
                table: "Jogos",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "EquipeVisitanteId",
                table: "Jogos",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Jogos_EquipeCasaId",
                table: "Jogos",
                column: "EquipeCasaId");

            migrationBuilder.CreateIndex(
                name: "IX_Jogos_EquipeVisitanteId",
                table: "Jogos",
                column: "EquipeVisitanteId");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Jogos_Equipes_EquipeCasaId",
                table: "Jogos");

            migrationBuilder.DropForeignKey(
                name: "FK_Jogos_Equipes_EquipeVisitanteId",
                table: "Jogos");

            migrationBuilder.DropIndex(
                name: "IX_Jogos_EquipeCasaId",
                table: "Jogos");

            migrationBuilder.DropIndex(
                name: "IX_Jogos_EquipeVisitanteId",
                table: "Jogos");

            migrationBuilder.DropColumn(
                name: "EquipeCasaId",
                table: "Jogos");

            migrationBuilder.DropColumn(
                name: "EquipeVisitanteId",
                table: "Jogos");
        }
    }
}
