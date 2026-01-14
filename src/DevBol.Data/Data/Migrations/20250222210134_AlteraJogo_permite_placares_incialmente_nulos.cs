using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevBol.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AlteraJogo_permite_placares_incialmente_nulos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            /*migrationBuilder.DropForeignKey(
                name: "FK_Apostadores_ApostadoresCampeonatos_ApostadorCampeonatoId",
                table: "Apostadores");*/

          /*  migrationBuilder.DropIndex(
                name: "IX_Apostadores_ApostadorCampeonatoId",
                table: "Apostadores");
*/
           /* migrationBuilder.DropColumn(
                name: "Ativa",
                table: "Rodadas");
*/
          /*  migrationBuilder.DropColumn(
                name: "ApostadorCampeonatoId",
                table: "Apostadores");*/

           /* migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Rodadas",
                type: "int",
                nullable: false,
                defaultValue: 0);
*/
            migrationBuilder.AlterColumn<int>(
                name: "PlacarVisita",
                table: "Jogos",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "PlacarCasa",
                table: "Jogos",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

         /*   migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Jogos",
                type: "int",
                nullable: false,
                defaultValue: 0);*/
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            /*migrationBuilder.DropColumn(
                name: "Status",
                table: "Rodadas");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Jogos");

            migrationBuilder.AddColumn<bool>(
                name: "Ativa",
                table: "Rodadas",
                type: "bit",
                nullable: false,
                defaultValue: false);*/

            migrationBuilder.AlterColumn<int>(
                name: "PlacarVisita",
                table: "Jogos",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "PlacarCasa",
                table: "Jogos",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            /*migrationBuilder.AddColumn<Guid>(
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
                principalColumn: "Id");*/
        }
    }
}
