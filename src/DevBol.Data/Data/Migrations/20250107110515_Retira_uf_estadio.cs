using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevBol.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class Retira_uf_estadio : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ufs_Estadios_EstadioId",
                table: "Ufs");

            migrationBuilder.DropIndex(
                name: "IX_Ufs_EstadioId",
                table: "Ufs");

            migrationBuilder.DropColumn(
                name: "EstadioId",
                table: "Ufs");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "EstadioId",
                table: "Ufs",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ufs_EstadioId",
                table: "Ufs",
                column: "EstadioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Ufs_Estadios_EstadioId",
                table: "Ufs",
                column: "EstadioId",
                principalTable: "Estadios",
                principalColumn: "Id");
        }
    }
}
