using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hotel.Infrastruture.Migrations
{
    /// <inheritdoc />
    public partial class alteraTableproduto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Produtos_PlanoDeContas_PlanoDeContasId",
                table: "Produtos");

            migrationBuilder.DropForeignKey(
                name: "FK_Produtos_ProductTypes_ProductTypesProductTypeCode",
                table: "Produtos");

            migrationBuilder.DropForeignKey(
                name: "FK_Produtos_TaxExemptionReasons_TaxExemptionReasonTaxExemptionCode",
                table: "Produtos");

            migrationBuilder.DropForeignKey(
                name: "FK_Produtos_TaxTableEntry_TaxTableEntryId",
                table: "Produtos");

            migrationBuilder.DropForeignKey(
                name: "FK_Produtos_TipoProdutos_TipoProdutosId",
                table: "Produtos");

            migrationBuilder.DropForeignKey(
                name: "FK_ProdutoStocks_Produtos_ProdutoId",
                table: "ProdutoStocks");

            migrationBuilder.DropForeignKey(
                name: "FK_Transferencias_Checkins_CheckinId",
                table: "Transferencias");

            migrationBuilder.DropForeignKey(
                name: "FK_Transferencias_Hospedagems_HospedagemDestinoId",
                table: "Transferencias");

            migrationBuilder.DropForeignKey(
                name: "FK_Transferencias_Hospedagems_HospedagemOrigemId",
                table: "Transferencias");

            migrationBuilder.DropForeignKey(
                name: "FK_Transferencias_MotivoTransferencias_MotivoTransferenciaId",
                table: "Transferencias");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Transferencias",
                table: "Transferencias");

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
                name: "MargemLucro",
                table: "Produtos");

            migrationBuilder.DropColumn(
                name: "Nome",
                table: "Produtos");

            migrationBuilder.DropColumn(
                name: "PrecoCompra",
                table: "Produtos");

            migrationBuilder.DropColumn(
                name: "Valor",
                table: "Produtos");

            migrationBuilder.RenameTable(
                name: "Transferencias",
                newName: "TransferenciaQuartos");

            migrationBuilder.RenameColumn(
                name: "TipoProdutosId",
                table: "Produtos",
                newName: "TipoProdutoId");

            migrationBuilder.RenameColumn(
                name: "TaxExemptionReasonTaxExemptionCode",
                table: "Produtos",
                newName: "TaxExemptionReasonCode");

            migrationBuilder.RenameColumn(
                name: "Quantidade",
                table: "Produtos",
                newName: "PontoDeVendasId");

            migrationBuilder.RenameColumn(
                name: "ProductTypesProductTypeCode",
                table: "Produtos",
                newName: "ProductTypeCode");

            migrationBuilder.RenameColumn(
                name: "PlanoDeContasId",
                table: "Produtos",
                newName: "PontoDeVendaId");

            migrationBuilder.RenameColumn(
                name: "EstoqueMinino",
                table: "Produtos",
                newName: "CategoriaId");

            migrationBuilder.RenameIndex(
                name: "IX_Produtos_TipoProdutosId",
                table: "Produtos",
                newName: "IX_Produtos_TipoProdutoId");

            migrationBuilder.RenameIndex(
                name: "IX_Produtos_TaxExemptionReasonTaxExemptionCode",
                table: "Produtos",
                newName: "IX_Produtos_TaxExemptionReasonCode");

            migrationBuilder.RenameIndex(
                name: "IX_Produtos_ProductTypesProductTypeCode",
                table: "Produtos",
                newName: "IX_Produtos_ProductTypeCode");

            migrationBuilder.RenameIndex(
                name: "IX_Produtos_PlanoDeContasId",
                table: "Produtos",
                newName: "IX_Produtos_PontoDeVendaId");

            migrationBuilder.RenameIndex(
                name: "IX_Transferencias_MotivoTransferenciaId",
                table: "TransferenciaQuartos",
                newName: "IX_TransferenciaQuartos_MotivoTransferenciaId");

            migrationBuilder.RenameIndex(
                name: "IX_Transferencias_HospedagemOrigemId",
                table: "TransferenciaQuartos",
                newName: "IX_TransferenciaQuartos_HospedagemOrigemId");

            migrationBuilder.RenameIndex(
                name: "IX_Transferencias_HospedagemDestinoId",
                table: "TransferenciaQuartos",
                newName: "IX_TransferenciaQuartos_HospedagemDestinoId");

            migrationBuilder.RenameIndex(
                name: "IX_Transferencias_CheckinId",
                table: "TransferenciaQuartos",
                newName: "IX_TransferenciaQuartos_CheckinId");

            migrationBuilder.AlterColumn<int>(
                name: "ProdutoId",
                table: "ProdutoStocks",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Observacoes",
                table: "ProdutoStocks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "QuantidadeMaxima",
                table: "ProdutoStocks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "QuantidadeMinima",
                table: "ProdutoStocks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "TaxTableEntryId",
                table: "Produtos",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PlanoDeContaId",
                table: "Produtos",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ManterPreco",
                table: "TransferenciaQuartos",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Observacao",
                table: "TransferenciaQuartos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UtilizadorId",
                table: "TransferenciaQuartos",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "ValorDiariaDestino",
                table: "TransferenciaQuartos",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "ValorDiariaOrigem",
                table: "TransferenciaQuartos",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddPrimaryKey(
                name: "PK_TransferenciaQuartos",
                table: "TransferenciaQuartos",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Categorias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CaminhoImagem = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IdTenant = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categorias", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "MotivoTransferencias",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "DateCreated", "IsActive" },
                values: new object[] { new DateTime(2025, 9, 12, 17, 19, 40, 148, DateTimeKind.Utc).AddTicks(3230), true });

            migrationBuilder.UpdateData(
                table: "MotivoTransferencias",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "DateCreated", "IsActive" },
                values: new object[] { new DateTime(2025, 9, 12, 17, 19, 40, 148, DateTimeKind.Utc).AddTicks(3230), true });

            migrationBuilder.UpdateData(
                table: "MotivoTransferencias",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "DateCreated", "IsActive" },
                values: new object[] { new DateTime(2025, 9, 12, 17, 19, 40, 148, DateTimeKind.Utc).AddTicks(3240), true });

            migrationBuilder.UpdateData(
                table: "MotivoTransferencias",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "DateCreated", "IsActive" },
                values: new object[] { new DateTime(2025, 9, 12, 17, 19, 40, 148, DateTimeKind.Utc).AddTicks(3240), true });

            migrationBuilder.CreateIndex(
                name: "IX_Produtos_CategoriaId",
                table: "Produtos",
                column: "CategoriaId");

            migrationBuilder.CreateIndex(
                name: "IX_Produtos_PlanoDeContaId",
                table: "Produtos",
                column: "PlanoDeContaId");

            migrationBuilder.CreateIndex(
                name: "IX_TransferenciaQuartos_UtilizadorId",
                table: "TransferenciaQuartos",
                column: "UtilizadorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Produtos_Categorias_CategoriaId",
                table: "Produtos",
                column: "CategoriaId",
                principalTable: "Categorias",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Produtos_PlanoDeContas_PlanoDeContaId",
                table: "Produtos",
                column: "PlanoDeContaId",
                principalTable: "PlanoDeContas",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Produtos_PonteDeVendas_PontoDeVendaId",
                table: "Produtos",
                column: "PontoDeVendaId",
                principalTable: "PonteDeVendas",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Produtos_ProductTypes_ProductTypeCode",
                table: "Produtos",
                column: "ProductTypeCode",
                principalTable: "ProductTypes",
                principalColumn: "ProductTypeCode");

            migrationBuilder.AddForeignKey(
                name: "FK_Produtos_TaxExemptionReasons_TaxExemptionReasonCode",
                table: "Produtos",
                column: "TaxExemptionReasonCode",
                principalTable: "TaxExemptionReasons",
                principalColumn: "TaxExemptionCode");

            migrationBuilder.AddForeignKey(
                name: "FK_Produtos_TaxTableEntry_TaxTableEntryId",
                table: "Produtos",
                column: "TaxTableEntryId",
                principalTable: "TaxTableEntry",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Produtos_TipoProdutos_TipoProdutoId",
                table: "Produtos",
                column: "TipoProdutoId",
                principalTable: "TipoProdutos",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProdutoStocks_Produtos_ProdutoId",
                table: "ProdutoStocks",
                column: "ProdutoId",
                principalTable: "Produtos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TransferenciaQuartos_Checkins_CheckinId",
                table: "TransferenciaQuartos",
                column: "CheckinId",
                principalTable: "Checkins",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

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

            migrationBuilder.AddForeignKey(
                name: "FK_TransferenciaQuartos_MotivoTransferencias_MotivoTransferenciaId",
                table: "TransferenciaQuartos",
                column: "MotivoTransferenciaId",
                principalTable: "MotivoTransferencias",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TransferenciaQuartos_Utilizador_UtilizadorId",
                table: "TransferenciaQuartos",
                column: "UtilizadorId",
                principalTable: "Utilizador",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Produtos_Categorias_CategoriaId",
                table: "Produtos");

            migrationBuilder.DropForeignKey(
                name: "FK_Produtos_PlanoDeContas_PlanoDeContaId",
                table: "Produtos");

            migrationBuilder.DropForeignKey(
                name: "FK_Produtos_PonteDeVendas_PontoDeVendaId",
                table: "Produtos");

            migrationBuilder.DropForeignKey(
                name: "FK_Produtos_ProductTypes_ProductTypeCode",
                table: "Produtos");

            migrationBuilder.DropForeignKey(
                name: "FK_Produtos_TaxExemptionReasons_TaxExemptionReasonCode",
                table: "Produtos");

            migrationBuilder.DropForeignKey(
                name: "FK_Produtos_TaxTableEntry_TaxTableEntryId",
                table: "Produtos");

            migrationBuilder.DropForeignKey(
                name: "FK_Produtos_TipoProdutos_TipoProdutoId",
                table: "Produtos");

            migrationBuilder.DropForeignKey(
                name: "FK_ProdutoStocks_Produtos_ProdutoId",
                table: "ProdutoStocks");

            migrationBuilder.DropForeignKey(
                name: "FK_TransferenciaQuartos_Checkins_CheckinId",
                table: "TransferenciaQuartos");

            migrationBuilder.DropForeignKey(
                name: "FK_TransferenciaQuartos_Hospedagems_HospedagemDestinoId",
                table: "TransferenciaQuartos");

            migrationBuilder.DropForeignKey(
                name: "FK_TransferenciaQuartos_Hospedagems_HospedagemOrigemId",
                table: "TransferenciaQuartos");

            migrationBuilder.DropForeignKey(
                name: "FK_TransferenciaQuartos_MotivoTransferencias_MotivoTransferenciaId",
                table: "TransferenciaQuartos");

            migrationBuilder.DropForeignKey(
                name: "FK_TransferenciaQuartos_Utilizador_UtilizadorId",
                table: "TransferenciaQuartos");

            migrationBuilder.DropTable(
                name: "Categorias");

            migrationBuilder.DropIndex(
                name: "IX_Produtos_CategoriaId",
                table: "Produtos");

            migrationBuilder.DropIndex(
                name: "IX_Produtos_PlanoDeContaId",
                table: "Produtos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TransferenciaQuartos",
                table: "TransferenciaQuartos");

            migrationBuilder.DropIndex(
                name: "IX_TransferenciaQuartos_UtilizadorId",
                table: "TransferenciaQuartos");

            migrationBuilder.DropColumn(
                name: "Observacoes",
                table: "ProdutoStocks");

            migrationBuilder.DropColumn(
                name: "QuantidadeMaxima",
                table: "ProdutoStocks");

            migrationBuilder.DropColumn(
                name: "QuantidadeMinima",
                table: "ProdutoStocks");

            migrationBuilder.DropColumn(
                name: "PlanoDeContaId",
                table: "Produtos");

            migrationBuilder.DropColumn(
                name: "ManterPreco",
                table: "TransferenciaQuartos");

            migrationBuilder.DropColumn(
                name: "Observacao",
                table: "TransferenciaQuartos");

            migrationBuilder.DropColumn(
                name: "UtilizadorId",
                table: "TransferenciaQuartos");

            migrationBuilder.DropColumn(
                name: "ValorDiariaDestino",
                table: "TransferenciaQuartos");

            migrationBuilder.DropColumn(
                name: "ValorDiariaOrigem",
                table: "TransferenciaQuartos");

            migrationBuilder.RenameTable(
                name: "TransferenciaQuartos",
                newName: "Transferencias");

            migrationBuilder.DropColumn(
                name: "TipoProdutoId",
                table: "Produtos");

            migrationBuilder.RenameColumn(
                name: "TaxExemptionReasonCode",
                table: "Produtos",
                newName: "TaxExemptionReasonTaxExemptionCode");

            migrationBuilder.RenameColumn(
                name: "ProductTypeCode",
                table: "Produtos",
                newName: "ProductTypesProductTypeCode");

            migrationBuilder.RenameColumn(
                name: "PontoDeVendasId",
                table: "Produtos",
                newName: "Quantidade");

            migrationBuilder.RenameColumn(
                name: "PontoDeVendaId",
                table: "Produtos",
                newName: "PlanoDeContasId");

            migrationBuilder.RenameColumn(
                name: "CategoriaId",
                table: "Produtos",
                newName: "EstoqueMinino");

            migrationBuilder.RenameIndex(
                name: "IX_Produtos_TipoProdutoId",
                table: "Produtos",
                newName: "IX_Produtos_TipoProdutosId");

            migrationBuilder.RenameIndex(
                name: "IX_Produtos_TaxExemptionReasonCode",
                table: "Produtos",
                newName: "IX_Produtos_TaxExemptionReasonTaxExemptionCode");

            migrationBuilder.RenameIndex(
                name: "IX_Produtos_ProductTypeCode",
                table: "Produtos",
                newName: "IX_Produtos_ProductTypesProductTypeCode");

            migrationBuilder.RenameIndex(
                name: "IX_Produtos_PontoDeVendaId",
                table: "Produtos",
                newName: "IX_Produtos_PlanoDeContasId");

            migrationBuilder.RenameIndex(
                name: "IX_TransferenciaQuartos_MotivoTransferenciaId",
                table: "Transferencias",
                newName: "IX_Transferencias_MotivoTransferenciaId");

            migrationBuilder.RenameIndex(
                name: "IX_TransferenciaQuartos_HospedagemOrigemId",
                table: "Transferencias",
                newName: "IX_Transferencias_HospedagemOrigemId");

            migrationBuilder.RenameIndex(
                name: "IX_TransferenciaQuartos_HospedagemDestinoId",
                table: "Transferencias",
                newName: "IX_Transferencias_HospedagemDestinoId");

            migrationBuilder.RenameIndex(
                name: "IX_TransferenciaQuartos_CheckinId",
                table: "Transferencias",
                newName: "IX_Transferencias_CheckinId");

            migrationBuilder.AlterColumn<int>(
                name: "ProdutoId",
                table: "ProdutoStocks",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "TaxTableEntryId",
                table: "Produtos",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<DateTime>(
                name: "DataExpiracao",
                table: "Produtos",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<float>(
                name: "Desconto",
                table: "Produtos",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "DescontoPercentagem",
                table: "Produtos",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "MargemLucro",
                table: "Produtos",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<string>(
                name: "Nome",
                table: "Produtos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "PrecoCompra",
                table: "Produtos",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "Valor",
                table: "Produtos",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Transferencias",
                table: "Transferencias",
                column: "Id");

            migrationBuilder.UpdateData(
                table: "MotivoTransferencias",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "DateCreated", "IsActive" },
                values: new object[] { new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false });

            migrationBuilder.UpdateData(
                table: "MotivoTransferencias",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "DateCreated", "IsActive" },
                values: new object[] { new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false });

            migrationBuilder.UpdateData(
                table: "MotivoTransferencias",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "DateCreated", "IsActive" },
                values: new object[] { new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false });

            migrationBuilder.UpdateData(
                table: "MotivoTransferencias",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "DateCreated", "IsActive" },
                values: new object[] { new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false });

            migrationBuilder.AddForeignKey(
                name: "FK_Produtos_PlanoDeContas_PlanoDeContasId",
                table: "Produtos",
                column: "PlanoDeContasId",
                principalTable: "PlanoDeContas",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Produtos_ProductTypes_ProductTypesProductTypeCode",
                table: "Produtos",
                column: "ProductTypesProductTypeCode",
                principalTable: "ProductTypes",
                principalColumn: "ProductTypeCode");

            migrationBuilder.AddForeignKey(
                name: "FK_Produtos_TaxExemptionReasons_TaxExemptionReasonTaxExemptionCode",
                table: "Produtos",
                column: "TaxExemptionReasonTaxExemptionCode",
                principalTable: "TaxExemptionReasons",
                principalColumn: "TaxExemptionCode");

            migrationBuilder.AddForeignKey(
                name: "FK_Produtos_TaxTableEntry_TaxTableEntryId",
                table: "Produtos",
                column: "TaxTableEntryId",
                principalTable: "TaxTableEntry",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Produtos_TipoProdutos_TipoProdutosId",
                table: "Produtos",
                column: "TipoProdutosId",
                principalTable: "TipoProdutos",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProdutoStocks_Produtos_ProdutoId",
                table: "ProdutoStocks",
                column: "ProdutoId",
                principalTable: "Produtos",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Transferencias_Checkins_CheckinId",
                table: "Transferencias",
                column: "CheckinId",
                principalTable: "Checkins",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Transferencias_Hospedagems_HospedagemDestinoId",
                table: "Transferencias",
                column: "HospedagemDestinoId",
                principalTable: "Hospedagems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Transferencias_Hospedagems_HospedagemOrigemId",
                table: "Transferencias",
                column: "HospedagemOrigemId",
                principalTable: "Hospedagems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Transferencias_MotivoTransferencias_MotivoTransferenciaId",
                table: "Transferencias",
                column: "MotivoTransferenciaId",
                principalTable: "MotivoTransferencias",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
