using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hotel.Infrastruture.Migrations
{
    /// <inheritdoc />
    public partial class alterandotabelatransferencia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Produtos_Categorias_CategoriaId",
                table: "Produtos");

            migrationBuilder.DropForeignKey(
                name: "FK_Produtos_PonteDeVendas_PontoDeVendasId",
                table: "Produtos");

            migrationBuilder.DropForeignKey(
                name: "FK_TransferenciaQuartos_Hospedagems_HospedagemDestinoId",
                table: "TransferenciaQuartos");

            migrationBuilder.DropForeignKey(
                name: "FK_TransferenciaQuartos_Hospedagems_HospedagemOrigemId",
                table: "TransferenciaQuartos");

            migrationBuilder.DropIndex(
                name: "IX_TransferenciaQuartos_HospedagemDestinoId",
                table: "TransferenciaQuartos");

            migrationBuilder.DropColumn(
                name: "HospedagemDestinoId",
                table: "TransferenciaQuartos");

            migrationBuilder.DropColumn(
                name: "ValorDiariaDestino",
                table: "TransferenciaQuartos");

            migrationBuilder.RenameColumn(
                name: "ValorDiariaOrigem",
                table: "TransferenciaQuartos",
                newName: "ValorDiaria");

            migrationBuilder.RenameColumn(
                name: "HospedagemOrigemId",
                table: "TransferenciaQuartos",
                newName: "QuartoId");

            migrationBuilder.RenameIndex(
                name: "IX_TransferenciaQuartos_HospedagemOrigemId",
                table: "TransferenciaQuartos",
                newName: "IX_TransferenciaQuartos_QuartoId");

            migrationBuilder.AddColumn<int>(
                name: "ApartamentosId",
                table: "TransferenciaQuartos",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DataEntrada",
                table: "TransferenciaQuartos",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DataSaida",
                table: "TransferenciaQuartos",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "HospedagemId",
                table: "TransferenciaQuartos",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HospedagemId1",
                table: "TransferenciaQuartos",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TipoTransferencia",
                table: "TransferenciaQuartos",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<decimal>(
                name: "ValorFixo",
                table: "Produtos",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AlterColumn<decimal>(
                name: "Valor",
                table: "Produtos",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AlterColumn<int>(
                name: "TaxTableEntryId",
                table: "Produtos",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<decimal>(
                name: "PrecoCIva",
                table: "Produtos",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AlterColumn<decimal>(
                name: "Lucro",
                table: "Produtos",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AlterColumn<int>(
                name: "AdicionarStock",
                table: "Produtos",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "CaminhoImagem",
                table: "Produtos",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DataExpiracao",
                table: "Produtos",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<decimal>(
                name: "Desconto",
                table: "Produtos",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DescontoPercentagem",
                table: "Produtos",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "EstoqueMinimo",
                table: "Produtos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "MargemLucro",
                table: "Produtos",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PrecoCompra",
                table: "Produtos",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "Quantidade",
                table: "Produtos",
                type: "int",
                nullable: false,
                defaultValue: 0);

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

            migrationBuilder.CreateIndex(
                name: "IX_TransferenciaQuartos_ApartamentosId",
                table: "TransferenciaQuartos",
                column: "ApartamentosId");

            migrationBuilder.CreateIndex(
                name: "IX_TransferenciaQuartos_HospedagemId",
                table: "TransferenciaQuartos",
                column: "HospedagemId");

            migrationBuilder.CreateIndex(
                name: "IX_TransferenciaQuartos_HospedagemId1",
                table: "TransferenciaQuartos",
                column: "HospedagemId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Produtos_Categorias_CategoriaId",
                table: "Produtos",
                column: "CategoriaId",
                principalTable: "Categorias",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Produtos_PonteDeVendas_PontoDeVendasId",
                table: "Produtos",
                column: "PontoDeVendasId",
                principalTable: "PonteDeVendas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TransferenciaQuartos_Apartamentos_ApartamentosId",
                table: "TransferenciaQuartos",
                column: "ApartamentosId",
                principalTable: "Apartamentos",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TransferenciaQuartos_Apartamentos_QuartoId",
                table: "TransferenciaQuartos",
                column: "QuartoId",
                principalTable: "Apartamentos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TransferenciaQuartos_Hospedagems_HospedagemId",
                table: "TransferenciaQuartos",
                column: "HospedagemId",
                principalTable: "Hospedagems",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TransferenciaQuartos_Hospedagems_HospedagemId1",
                table: "TransferenciaQuartos",
                column: "HospedagemId1",
                principalTable: "Hospedagems",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Produtos_Categorias_CategoriaId",
                table: "Produtos");

            migrationBuilder.DropForeignKey(
                name: "FK_Produtos_PonteDeVendas_PontoDeVendasId",
                table: "Produtos");

            migrationBuilder.DropForeignKey(
                name: "FK_TransferenciaQuartos_Apartamentos_ApartamentosId",
                table: "TransferenciaQuartos");

            migrationBuilder.DropForeignKey(
                name: "FK_TransferenciaQuartos_Apartamentos_QuartoId",
                table: "TransferenciaQuartos");

            migrationBuilder.DropForeignKey(
                name: "FK_TransferenciaQuartos_Hospedagems_HospedagemId",
                table: "TransferenciaQuartos");

            migrationBuilder.DropForeignKey(
                name: "FK_TransferenciaQuartos_Hospedagems_HospedagemId1",
                table: "TransferenciaQuartos");

            migrationBuilder.DropIndex(
                name: "IX_TransferenciaQuartos_ApartamentosId",
                table: "TransferenciaQuartos");

            migrationBuilder.DropIndex(
                name: "IX_TransferenciaQuartos_HospedagemId",
                table: "TransferenciaQuartos");

            migrationBuilder.DropIndex(
                name: "IX_TransferenciaQuartos_HospedagemId1",
                table: "TransferenciaQuartos");

            migrationBuilder.DropColumn(
                name: "ApartamentosId",
                table: "TransferenciaQuartos");

            migrationBuilder.DropColumn(
                name: "DataEntrada",
                table: "TransferenciaQuartos");

            migrationBuilder.DropColumn(
                name: "DataSaida",
                table: "TransferenciaQuartos");

            migrationBuilder.DropColumn(
                name: "HospedagemId",
                table: "TransferenciaQuartos");

            migrationBuilder.DropColumn(
                name: "HospedagemId1",
                table: "TransferenciaQuartos");

            migrationBuilder.DropColumn(
                name: "TipoTransferencia",
                table: "TransferenciaQuartos");

            migrationBuilder.DropColumn(
                name: "CaminhoImagem",
                table: "Produtos");

            migrationBuilder.DropColumn(
                name: "DataExpiracao",
                table: "Produtos");

            migrationBuilder.DropColumn(
                name: "Desconto",
                table: "Produtos");

            migrationBuilder.DropColumn(
                name: "DescontoPercentagem",
                table: "Produtos");

            migrationBuilder.DropColumn(
                name: "EstoqueMinimo",
                table: "Produtos");

            migrationBuilder.DropColumn(
                name: "MargemLucro",
                table: "Produtos");

            migrationBuilder.DropColumn(
                name: "PrecoCompra",
                table: "Produtos");

            migrationBuilder.DropColumn(
                name: "Quantidade",
                table: "Produtos");

            migrationBuilder.RenameColumn(
                name: "ValorDiaria",
                table: "TransferenciaQuartos",
                newName: "ValorDiariaOrigem");

            migrationBuilder.RenameColumn(
                name: "QuartoId",
                table: "TransferenciaQuartos",
                newName: "HospedagemOrigemId");

            migrationBuilder.RenameIndex(
                name: "IX_TransferenciaQuartos_QuartoId",
                table: "TransferenciaQuartos",
                newName: "IX_TransferenciaQuartos_HospedagemOrigemId");

            migrationBuilder.AddColumn<int>(
                name: "HospedagemDestinoId",
                table: "TransferenciaQuartos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<float>(
                name: "ValorDiariaDestino",
                table: "TransferenciaQuartos",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AlterColumn<float>(
                name: "ValorFixo",
                table: "Produtos",
                type: "real",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldDefaultValue: 0m);

            migrationBuilder.AlterColumn<float>(
                name: "Valor",
                table: "Produtos",
                type: "real",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<int>(
                name: "TaxTableEntryId",
                table: "Produtos",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 0);

            migrationBuilder.AlterColumn<float>(
                name: "PrecoCIva",
                table: "Produtos",
                type: "real",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldDefaultValue: 0m);

            migrationBuilder.AlterColumn<float>(
                name: "Lucro",
                table: "Produtos",
                type: "real",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldDefaultValue: 0m);

            migrationBuilder.AlterColumn<int>(
                name: "AdicionarStock",
                table: "Produtos",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 0);

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
                name: "IX_TransferenciaQuartos_HospedagemDestinoId",
                table: "TransferenciaQuartos",
                column: "HospedagemDestinoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Produtos_Categorias_CategoriaId",
                table: "Produtos",
                column: "CategoriaId",
                principalTable: "Categorias",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Produtos_PonteDeVendas_PontoDeVendasId",
                table: "Produtos",
                column: "PontoDeVendasId",
                principalTable: "PonteDeVendas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TransferenciaQuartos_Hospedagems_HospedagemDestinoId",
                table: "TransferenciaQuartos",
                column: "HospedagemDestinoId",
                principalTable: "Hospedagems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TransferenciaQuartos_Hospedagems_HospedagemOrigemId",
                table: "TransferenciaQuartos",
                column: "HospedagemOrigemId",
                principalTable: "Hospedagems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
