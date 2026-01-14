using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevBol.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AlteraTamanhoCPF_Apostador : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CPF",
                table: "Apostadores",
                type: "varchar(11)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(14)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CPF",
                table: "Apostadores",
                type: "varchar(14)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(11)");
        }
    }
}
