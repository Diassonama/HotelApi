using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hotel.Infrastruture.Migrations
{
    /// <inheritdoc />
    public partial class removerTipolancamentoId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "TipoLancamento",
                table: "EmpresaSaldoMovimentos",
                type: "varchar(1)",
                nullable: false,
                defaultValue: "E",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.UpdateData(
                table: "MotivoTransferencias",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateCreated",
                value: new DateTime(2026, 1, 23, 3, 20, 0, 647, DateTimeKind.Utc).AddTicks(570));

            migrationBuilder.UpdateData(
                table: "MotivoTransferencias",
                keyColumn: "Id",
                keyValue: 2,
                column: "DateCreated",
                value: new DateTime(2026, 1, 23, 3, 20, 0, 647, DateTimeKind.Utc).AddTicks(570));

            migrationBuilder.UpdateData(
                table: "MotivoTransferencias",
                keyColumn: "Id",
                keyValue: 3,
                column: "DateCreated",
                value: new DateTime(2026, 1, 23, 3, 20, 0, 647, DateTimeKind.Utc).AddTicks(570));

            migrationBuilder.UpdateData(
                table: "MotivoTransferencias",
                keyColumn: "Id",
                keyValue: 4,
                column: "DateCreated",
                value: new DateTime(2026, 1, 23, 3, 20, 0, 647, DateTimeKind.Utc).AddTicks(570));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "TipoLancamento",
                table: "EmpresaSaldoMovimentos",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(1)",
                oldDefaultValue: "E");

            migrationBuilder.UpdateData(
                table: "MotivoTransferencias",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateCreated",
                value: new DateTime(2026, 1, 19, 21, 34, 16, 495, DateTimeKind.Utc).AddTicks(2450));

            migrationBuilder.UpdateData(
                table: "MotivoTransferencias",
                keyColumn: "Id",
                keyValue: 2,
                column: "DateCreated",
                value: new DateTime(2026, 1, 19, 21, 34, 16, 495, DateTimeKind.Utc).AddTicks(2450));

            migrationBuilder.UpdateData(
                table: "MotivoTransferencias",
                keyColumn: "Id",
                keyValue: 3,
                column: "DateCreated",
                value: new DateTime(2026, 1, 19, 21, 34, 16, 495, DateTimeKind.Utc).AddTicks(2460));

            migrationBuilder.UpdateData(
                table: "MotivoTransferencias",
                keyColumn: "Id",
                keyValue: 4,
                column: "DateCreated",
                value: new DateTime(2026, 1, 19, 21, 34, 16, 495, DateTimeKind.Utc).AddTicks(2460));
        }
    }
}
