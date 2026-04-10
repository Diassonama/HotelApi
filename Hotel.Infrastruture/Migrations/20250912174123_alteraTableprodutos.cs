using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hotel.Infrastruture.Migrations
{
    /// <inheritdoc />
    public partial class alteraTableprodutos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Produtos_PonteDeVendas_PontoDeVendaId",
                table: "Produtos");

            migrationBuilder.DropIndex(
                name: "IX_Produtos_PontoDeVendaId",
                table: "Produtos");

            migrationBuilder.DropColumn(
                name: "PontoDeVendaId",
                table: "Produtos");

            migrationBuilder.AddColumn<string>(
                name: "Nome",
                table: "Produtos",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<float>(
                name: "Valor",
                table: "Produtos",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.UpdateData(
                table: "MotivoTransferencias",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateCreated",
                value: new DateTime(2025, 9, 12, 17, 41, 22, 920, DateTimeKind.Utc).AddTicks(6460));

            migrationBuilder.UpdateData(
                table: "MotivoTransferencias",
                keyColumn: "Id",
                keyValue: 2,
                column: "DateCreated",
                value: new DateTime(2025, 9, 12, 17, 41, 22, 920, DateTimeKind.Utc).AddTicks(6460));

            migrationBuilder.UpdateData(
                table: "MotivoTransferencias",
                keyColumn: "Id",
                keyValue: 3,
                column: "DateCreated",
                value: new DateTime(2025, 9, 12, 17, 41, 22, 920, DateTimeKind.Utc).AddTicks(6460));

            migrationBuilder.UpdateData(
                table: "MotivoTransferencias",
                keyColumn: "Id",
                keyValue: 4,
                column: "DateCreated",
                value: new DateTime(2025, 9, 12, 17, 41, 22, 920, DateTimeKind.Utc).AddTicks(6460));

            migrationBuilder.CreateIndex(
                name: "IX_Produtos_PontoDeVendasId",
                table: "Produtos",
                column: "PontoDeVendasId");

            migrationBuilder.AddForeignKey(
                name: "FK_Produtos_PonteDeVendas_PontoDeVendasId",
                table: "Produtos",
                column: "PontoDeVendasId",
                principalTable: "PonteDeVendas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Produtos_PonteDeVendas_PontoDeVendasId",
                table: "Produtos");

            migrationBuilder.DropIndex(
                name: "IX_Produtos_PontoDeVendasId",
                table: "Produtos");

            migrationBuilder.DropColumn(
                name: "Nome",
                table: "Produtos");

            migrationBuilder.DropColumn(
                name: "Valor",
                table: "Produtos");

            migrationBuilder.AddColumn<int>(
                name: "PontoDeVendaId",
                table: "Produtos",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "MotivoTransferencias",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateCreated",
                value: new DateTime(2025, 9, 12, 17, 19, 40, 148, DateTimeKind.Utc).AddTicks(3230));

            migrationBuilder.UpdateData(
                table: "MotivoTransferencias",
                keyColumn: "Id",
                keyValue: 2,
                column: "DateCreated",
                value: new DateTime(2025, 9, 12, 17, 19, 40, 148, DateTimeKind.Utc).AddTicks(3230));

            migrationBuilder.UpdateData(
                table: "MotivoTransferencias",
                keyColumn: "Id",
                keyValue: 3,
                column: "DateCreated",
                value: new DateTime(2025, 9, 12, 17, 19, 40, 148, DateTimeKind.Utc).AddTicks(3240));

            migrationBuilder.UpdateData(
                table: "MotivoTransferencias",
                keyColumn: "Id",
                keyValue: 4,
                column: "DateCreated",
                value: new DateTime(2025, 9, 12, 17, 19, 40, 148, DateTimeKind.Utc).AddTicks(3240));

            migrationBuilder.CreateIndex(
                name: "IX_Produtos_PontoDeVendaId",
                table: "Produtos",
                column: "PontoDeVendaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Produtos_PonteDeVendas_PontoDeVendaId",
                table: "Produtos",
                column: "PontoDeVendaId",
                principalTable: "PonteDeVendas",
                principalColumn: "Id");
        }
    }
}
