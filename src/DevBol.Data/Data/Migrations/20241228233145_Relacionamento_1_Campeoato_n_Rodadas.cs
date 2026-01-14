using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevBol.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class Relacionamento_1_Campeoato_n_Rodadas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CampeonatoId",
                table: "Rodadas",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Rodadas_CampeonatoId",
                table: "Rodadas",
                column: "CampeonatoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Rodadas_Campeonatos_CampeonatoId",
                table: "Rodadas",
                column: "CampeonatoId",
                principalTable: "Campeonatos",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rodadas_Campeonatos_CampeonatoId",
                table: "Rodadas");

            migrationBuilder.DropIndex(
                name: "IX_Rodadas_CampeonatoId",
                table: "Rodadas");

            migrationBuilder.DropColumn(
                name: "CampeonatoId",
                table: "Rodadas");
        }
    }
}
