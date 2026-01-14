using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevBol.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class Ajuste_chaves_ApostadoresDoCampeonato_Apostas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApostadoresCampeonatos_ApostadoresCampeonatos_ApostadorCampeonatoId",
                table: "ApostadoresCampeonatos");

            migrationBuilder.DropIndex(
                name: "IX_ApostadoresCampeonatos_ApostadorCampeonatoId",
                table: "ApostadoresCampeonatos");

            migrationBuilder.DropColumn(
                name: "ApostadorCampeonatoId",
                table: "ApostadoresCampeonatos");

            migrationBuilder.AddColumn<Guid>(
                name: "ApostadorCampeonatoId",
                table: "Apostadores",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Apostadores_ApostadorCampeonatoId",
                table: "Apostadores",
                column: "ApostadorCampeonatoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Apostadores_ApostadoresCampeonatos_ApostadorCampeonatoId",
                table: "Apostadores",
                column: "ApostadorCampeonatoId",
                principalTable: "ApostadoresCampeonatos",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Apostadores_ApostadoresCampeonatos_ApostadorCampeonatoId",
                table: "Apostadores");

            migrationBuilder.DropIndex(
                name: "IX_Apostadores_ApostadorCampeonatoId",
                table: "Apostadores");

            migrationBuilder.DropColumn(
                name: "ApostadorCampeonatoId",
                table: "Apostadores");

            migrationBuilder.AddColumn<Guid>(
                name: "ApostadorCampeonatoId",
                table: "ApostadoresCampeonatos",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ApostadoresCampeonatos_ApostadorCampeonatoId",
                table: "ApostadoresCampeonatos",
                column: "ApostadorCampeonatoId");

            migrationBuilder.AddForeignKey(
                name: "FK_ApostadoresCampeonatos_ApostadoresCampeonatos_ApostadorCampeonatoId",
                table: "ApostadoresCampeonatos",
                column: "ApostadorCampeonatoId",
                principalTable: "ApostadoresCampeonatos",
                principalColumn: "Id");
        }
    }
}
