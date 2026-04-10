using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hotel.Infrastruture.Migrations
{
    /// <inheritdoc />
    public partial class alterandotabelatransferencia2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "ValorDiaria",
                table: "TransferenciaQuartos",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AlterColumn<string>(
                name: "UtilizadorId",
                table: "TransferenciaQuartos",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Observacao",
                table: "TransferenciaQuartos",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "ManterPreco",
                table: "TransferenciaQuartos",
                type: "bit",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "bit");

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

            migrationBuilder.AddColumn<string>(
                name: "UtilizadorId1",
                table: "TransferenciaQuartos",
                type: "nvarchar(450)",
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

            migrationBuilder.CreateIndex(
                name: "IX_TransferenciaQuartos_UtilizadorId1",
                table: "TransferenciaQuartos",
                column: "UtilizadorId1");

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

            migrationBuilder.AddForeignKey(
                name: "FK_TransferenciaQuartos_Utilizador_UtilizadorId1",
                table: "TransferenciaQuartos",
                column: "UtilizadorId1",
                principalTable: "Utilizador",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TransferenciaQuartos_Apartamentos_ApartamentosId1",
                table: "TransferenciaQuartos");

            migrationBuilder.DropForeignKey(
                name: "FK_TransferenciaQuartos_Checkins_CheckinsId",
                table: "TransferenciaQuartos");

            migrationBuilder.DropForeignKey(
                name: "FK_TransferenciaQuartos_MotivoTransferencias_MotivoTransferenciaId1",
                table: "TransferenciaQuartos");

            migrationBuilder.DropForeignKey(
                name: "FK_TransferenciaQuartos_Utilizador_UtilizadorId1",
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

            migrationBuilder.DropIndex(
                name: "IX_TransferenciaQuartos_UtilizadorId1",
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

            migrationBuilder.DropColumn(
                name: "UtilizadorId1",
                table: "TransferenciaQuartos");

            migrationBuilder.AlterColumn<float>(
                name: "ValorDiaria",
                table: "TransferenciaQuartos",
                type: "real",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<string>(
                name: "UtilizadorId",
                table: "TransferenciaQuartos",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldMaxLength: 450);

            migrationBuilder.AlterColumn<string>(
                name: "Observacao",
                table: "TransferenciaQuartos",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "ManterPreco",
                table: "TransferenciaQuartos",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: true);

            migrationBuilder.UpdateData(
                table: "MotivoTransferencias",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateCreated",
                value: new DateTime(2025, 9, 17, 22, 46, 26, 640, DateTimeKind.Utc).AddTicks(6510));

            migrationBuilder.UpdateData(
                table: "MotivoTransferencias",
                keyColumn: "Id",
                keyValue: 2,
                column: "DateCreated",
                value: new DateTime(2025, 9, 17, 22, 46, 26, 640, DateTimeKind.Utc).AddTicks(6520));

            migrationBuilder.UpdateData(
                table: "MotivoTransferencias",
                keyColumn: "Id",
                keyValue: 3,
                column: "DateCreated",
                value: new DateTime(2025, 9, 17, 22, 46, 26, 640, DateTimeKind.Utc).AddTicks(6520));

            migrationBuilder.UpdateData(
                table: "MotivoTransferencias",
                keyColumn: "Id",
                keyValue: 4,
                column: "DateCreated",
                value: new DateTime(2025, 9, 17, 22, 46, 26, 640, DateTimeKind.Utc).AddTicks(6520));
        }
    }
}
