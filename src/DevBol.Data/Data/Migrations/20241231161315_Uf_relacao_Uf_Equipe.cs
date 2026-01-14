using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevBol.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class Uf_relacao_Uf_Equipe : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UfId",
                table: "Equipes",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "Ufs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nome = table.Column<string>(type: "varchar(40)", nullable: false),
                    Sigla = table.Column<string>(type: "varchar(2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ufs", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Equipes_UfId",
                table: "Equipes",
                column: "UfId");

            migrationBuilder.AddForeignKey(
                name: "FK_Equipes_Ufs_UfId",
                table: "Equipes",
                column: "UfId",
                principalTable: "Ufs",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Equipes_Ufs_UfId",
                table: "Equipes");

            migrationBuilder.DropTable(
                name: "Ufs");

            migrationBuilder.DropIndex(
                name: "IX_Equipes_UfId",
                table: "Equipes");

            migrationBuilder.DropColumn(
                name: "UfId",
                table: "Equipes");
        }
    }
}
