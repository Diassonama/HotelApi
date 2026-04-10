using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hotel.Infrastruture.Migrations
{
    /// <inheritdoc />
    public partial class alterandotabelatransferencoluna2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TransferenciaQuartos_Apartamentos_ApartamentosId",
                table: "TransferenciaQuartos");

            migrationBuilder.DropForeignKey(
                name: "FK_TransferenciaQuartos_Apartamentos_ApartamentosId1",
                table: "TransferenciaQuartos");

            migrationBuilder.DropForeignKey(
                name: "FK_TransferenciaQuartos_Checkins_CheckinsId",
                table: "TransferenciaQuartos");

            migrationBuilder.DropForeignKey(
                name: "FK_TransferenciaQuartos_MotivoTransferencias_MotivoTransferenciaId1",
                table: "TransferenciaQuartos");

            migrationBuilder.DropIndex(
                name: "IX_TransferenciaQuartos_ApartamentosId",
                table: "TransferenciaQuartos");

            migrationBuilder.DropIndex(
                name: "IX_TransferenciaQuartos_ApartamentosId1",
                table: "TransferenciaQuartos");

            migrationBuilder.DropIndex(
                name: "IX_TransferenciaQuartos_CheckinsId",
                table: "TransferenciaQuartos");

            migrationBuilder.DropIndex(
                name: "IX_TransferenciaQuartos_MotivoTransferenciaId1",
                table: "TransferenciaQuartos");

            migrationBuilder.DropColumn(
                name: "ApartamentosId",
                table: "TransferenciaQuartos");

            migrationBuilder.DropColumn(
                name: "ApartamentosId1",
                table: "TransferenciaQuartos");

            migrationBuilder.DropColumn(
                name: "CheckinsId",
                table: "TransferenciaQuartos");

            migrationBuilder.DropColumn(
                name: "MotivoTransferenciaId1",
                table: "TransferenciaQuartos");

            migrationBuilder.UpdateData(
                table: "MotivoTransferencias",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateCreated",
                value: new DateTime(2025, 9, 17, 23, 10, 8, 714, DateTimeKind.Utc).AddTicks(6320));

            migrationBuilder.UpdateData(
                table: "MotivoTransferencias",
                keyColumn: "Id",
                keyValue: 2,
                column: "DateCreated",
                value: new DateTime(2025, 9, 17, 23, 10, 8, 714, DateTimeKind.Utc).AddTicks(6320));

            migrationBuilder.UpdateData(
                table: "MotivoTransferencias",
                keyColumn: "Id",
                keyValue: 3,
                column: "DateCreated",
                value: new DateTime(2025, 9, 17, 23, 10, 8, 714, DateTimeKind.Utc).AddTicks(6320));

            migrationBuilder.UpdateData(
                table: "MotivoTransferencias",
                keyColumn: "Id",
                keyValue: 4,
                column: "DateCreated",
                value: new DateTime(2025, 9, 17, 23, 10, 8, 714, DateTimeKind.Utc).AddTicks(6320));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ApartamentosId",
                table: "TransferenciaQuartos",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ApartamentosId1",
                table: "TransferenciaQuartos",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CheckinsId",
                table: "TransferenciaQuartos",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MotivoTransferenciaId1",
                table: "TransferenciaQuartos",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "MotivoTransferencias",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateCreated",
                value: new DateTime(2025, 9, 17, 22, 55, 2, 886, DateTimeKind.Utc).AddTicks(9080));

            migrationBuilder.UpdateData(
                table: "MotivoTransferencias",
                keyColumn: "Id",
                keyValue: 2,
                column: "DateCreated",
                value: new DateTime(2025, 9, 17, 22, 55, 2, 886, DateTimeKind.Utc).AddTicks(9080));

            migrationBuilder.UpdateData(
                table: "MotivoTransferencias",
                keyColumn: "Id",
                keyValue: 3,
                column: "DateCreated",
                value: new DateTime(2025, 9, 17, 22, 55, 2, 886, DateTimeKind.Utc).AddTicks(9080));

            migrationBuilder.UpdateData(
                table: "MotivoTransferencias",
                keyColumn: "Id",
                keyValue: 4,
                column: "DateCreated",
                value: new DateTime(2025, 9, 17, 22, 55, 2, 886, DateTimeKind.Utc).AddTicks(9080));

            migrationBuilder.CreateIndex(
                name: "IX_TransferenciaQuartos_ApartamentosId",
                table: "TransferenciaQuartos",
                column: "ApartamentosId");

            migrationBuilder.CreateIndex(
                name: "IX_TransferenciaQuartos_ApartamentosId1",
                table: "TransferenciaQuartos",
                column: "ApartamentosId1");

            migrationBuilder.CreateIndex(
                name: "IX_TransferenciaQuartos_CheckinsId",
                table: "TransferenciaQuartos",
                column: "CheckinsId");

            migrationBuilder.CreateIndex(
                name: "IX_TransferenciaQuartos_MotivoTransferenciaId1",
                table: "TransferenciaQuartos",
                column: "MotivoTransferenciaId1");

            migrationBuilder.AddForeignKey(
                name: "FK_TransferenciaQuartos_Apartamentos_ApartamentosId",
                table: "TransferenciaQuartos",
                column: "ApartamentosId",
                principalTable: "Apartamentos",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TransferenciaQuartos_Apartamentos_ApartamentosId1",
                table: "TransferenciaQuartos",
                column: "ApartamentosId1",
                principalTable: "Apartamentos",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TransferenciaQuartos_Checkins_CheckinsId",
                table: "TransferenciaQuartos",
                column: "CheckinsId",
                principalTable: "Checkins",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TransferenciaQuartos_MotivoTransferencias_MotivoTransferenciaId1",
                table: "TransferenciaQuartos",
                column: "MotivoTransferenciaId1",
                principalTable: "MotivoTransferencias",
                principalColumn: "Id");
        }
    }
}
