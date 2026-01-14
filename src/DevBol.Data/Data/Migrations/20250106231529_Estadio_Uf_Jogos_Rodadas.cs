using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevBol.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class Estadio_Uf_Jogos_Rodadas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "EstadioId",
                table: "Ufs",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Estadios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nome = table.Column<string>(type: "varchar(60)", nullable: false),
                    UfId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Estadios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Estadios_Ufs_UfId",
                        column: x => x.UfId,
                        principalTable: "Ufs",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Jogos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DataJogo = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HoraJogo = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PlacarCasa = table.Column<int>(type: "int", nullable: false),
                    PlacarVisita = table.Column<int>(type: "int", nullable: false),
                    RodadaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EstadioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Jogos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Jogos_Estadios_EstadioId",
                        column: x => x.EstadioId,
                        principalTable: "Estadios",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Jogos_Rodadas_RodadaId",
                        column: x => x.RodadaId,
                        principalTable: "Rodadas",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Ufs_EstadioId",
                table: "Ufs",
                column: "EstadioId");

            migrationBuilder.CreateIndex(
                name: "IX_Estadios_UfId",
                table: "Estadios",
                column: "UfId");

            migrationBuilder.CreateIndex(
                name: "IX_Jogos_EstadioId",
                table: "Jogos",
                column: "EstadioId");

            migrationBuilder.CreateIndex(
                name: "IX_Jogos_RodadaId",
                table: "Jogos",
                column: "RodadaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Ufs_Estadios_EstadioId",
                table: "Ufs",
                column: "EstadioId",
                principalTable: "Estadios",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ufs_Estadios_EstadioId",
                table: "Ufs");

            migrationBuilder.DropTable(
                name: "Jogos");

            migrationBuilder.DropTable(
                name: "Estadios");

            migrationBuilder.DropIndex(
                name: "IX_Ufs_EstadioId",
                table: "Ufs");

            migrationBuilder.DropColumn(
                name: "EstadioId",
                table: "Ufs");
        }
    }
}
