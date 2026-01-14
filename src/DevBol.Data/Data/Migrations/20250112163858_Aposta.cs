using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevBol.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class Aposta : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ApostadorCampeonatoId",
                table: "ApostadoresCampeonatos",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Apostas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    JogoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApostadorCampeonatoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DataHoraAposta = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PlacarApostaCasa = table.Column<int>(type: "int", nullable: false),
                    PlacarApostaVisita = table.Column<int>(type: "int", nullable: false),
                    Enviada = table.Column<bool>(type: "bit", nullable: false),
                    Pontos = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Apostas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Apostas_ApostadoresCampeonatos_ApostadorCampeonatoId",
                        column: x => x.ApostadorCampeonatoId,
                        principalTable: "ApostadoresCampeonatos",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Apostas_Jogos_JogoId",
                        column: x => x.JogoId,
                        principalTable: "Jogos",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApostadoresCampeonatos_ApostadorCampeonatoId",
                table: "ApostadoresCampeonatos",
                column: "ApostadorCampeonatoId");

            migrationBuilder.CreateIndex(
                name: "IX_Apostas_ApostadorCampeonatoId",
                table: "Apostas",
                column: "ApostadorCampeonatoId");

            migrationBuilder.CreateIndex(
                name: "IX_Apostas_JogoId",
                table: "Apostas",
                column: "JogoId");

            migrationBuilder.AddForeignKey(
                name: "FK_ApostadoresCampeonatos_ApostadoresCampeonatos_ApostadorCampeonatoId",
                table: "ApostadoresCampeonatos",
                column: "ApostadorCampeonatoId",
                principalTable: "ApostadoresCampeonatos",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApostadoresCampeonatos_ApostadoresCampeonatos_ApostadorCampeonatoId",
                table: "ApostadoresCampeonatos");

            migrationBuilder.DropTable(
                name: "Apostas");

            migrationBuilder.DropIndex(
                name: "IX_ApostadoresCampeonatos_ApostadorCampeonatoId",
                table: "ApostadoresCampeonatos");

            migrationBuilder.DropColumn(
                name: "ApostadorCampeonatoId",
                table: "ApostadoresCampeonatos");
        }
    }
}
