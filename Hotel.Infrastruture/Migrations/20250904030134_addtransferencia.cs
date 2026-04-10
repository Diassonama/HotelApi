using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Hotel.Infrastruture.Migrations
{
    /// <inheritdoc />
    public partial class addtransferencia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItemPedidos_Pedidos_PedidosId",
                table: "ItemPedidos");

            migrationBuilder.DropForeignKey(
                name: "FK_Pedidos_Hospedes_HospedesId",
                table: "Pedidos");

            migrationBuilder.DropForeignKey(
                name: "FK_Pedidos_PonteDeVendas_PonteDeVendasId",
                table: "Pedidos");

            migrationBuilder.DropIndex(
                name: "IX_ItemPedidos_PedidosId",
                table: "ItemPedidos");

            migrationBuilder.DropColumn(
                name: "Valor",
                table: "Pedidos");

            migrationBuilder.DropColumn(
                name: "PedidosId",
                table: "ItemPedidos");

            migrationBuilder.DropColumn(
                name: "Preco",
                table: "ItemPedidos");

            migrationBuilder.RenameColumn(
                name: "PonteDeVendasId",
                table: "Pedidos",
                newName: "PontoVendaId");

            migrationBuilder.RenameColumn(
                name: "HospedesId",
                table: "Pedidos",
                newName: "HospedeId");

            migrationBuilder.RenameIndex(
                name: "IX_Pedidos_PonteDeVendasId",
                table: "Pedidos",
                newName: "IX_Pedidos_PontoVendaId");

            migrationBuilder.RenameIndex(
                name: "IX_Pedidos_HospedesId",
                table: "Pedidos",
                newName: "IX_Pedidos_HospedeId");

            migrationBuilder.RenameColumn(
                name: "IdProduto",
                table: "ItemPedidos",
                newName: "ProdutoId");

            migrationBuilder.AlterColumn<int>(
                name: "SituacaoPagamento",
                table: "Pedidos",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DataFinalizacao",
                table: "Pedidos",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DataPedido",
                table: "Pedidos",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "NumePedido",
                table: "Pedidos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Categoria",
                table: "ItemPedidos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NomeProduto",
                table: "ItemPedidos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Observacao",
                table: "ItemPedidos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PedidoId",
                table: "ItemPedidos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "PrecoUnitario",
                table: "ItemPedidos",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DataFechamento",
                table: "Hospedagems",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.CreateTable(
                name: "MotivoTransferencias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IdTenant = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MotivoTransferencias", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Transferencias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CheckinId = table.Column<int>(type: "int", nullable: false),
                    MotivoTransferenciaId = table.Column<int>(type: "int", nullable: false),
                    HospedagemOrigemId = table.Column<int>(type: "int", nullable: false),
                    HospedagemDestinoId = table.Column<int>(type: "int", nullable: false),
                    DataTransferencia = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transferencias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transferencias_Checkins_CheckinId",
                        column: x => x.CheckinId,
                        principalTable: "Checkins",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Transferencias_Hospedagems_HospedagemDestinoId",
                        column: x => x.HospedagemDestinoId,
                        principalTable: "Hospedagems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Transferencias_Hospedagems_HospedagemOrigemId",
                        column: x => x.HospedagemOrigemId,
                        principalTable: "Hospedagems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Transferencias_MotivoTransferencias_MotivoTransferenciaId",
                        column: x => x.MotivoTransferenciaId,
                        principalTable: "MotivoTransferencias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "MotivoTransferencias",
                columns: new[] { "Id", "CreatedBy", "DateCreated", "Descricao", "IdTenant", "IsActive", "LastModifiedDate" },
                values: new object[,]
                {
                    { 1, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Manutenção", 0, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Upgrade", 0, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 3, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Preferência do Cliente", 0, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 4, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Outros", 0, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.UpdateData(
                table: "TipoApartamentos",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Domingo", "Quarta", "Quinta", "Sabado", "Segunda", "Sexta", "Terca" },
                values: new object[] { 10000f, 10000f, 10000f, 10000f, 10000f, 10000f, 10000f });

            migrationBuilder.UpdateData(
                table: "TipoApartamentos",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Domingo", "Quarta", "Quinta", "Sabado", "Segunda", "Sexta", "Terca" },
                values: new object[] { 10000f, 10000f, 10000f, 10000f, 10000f, 10000f, 10000f });

            migrationBuilder.UpdateData(
                table: "TipoApartamentos",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Domingo", "Quarta", "Quinta", "Sabado", "Segunda", "Sexta", "Terca" },
                values: new object[] { 10000f, 10000f, 10000f, 10000f, 10000f, 10000f, 10000f });

            migrationBuilder.UpdateData(
                table: "TipoApartamentos",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Domingo", "Quarta", "Quinta", "Sabado", "Segunda", "Sexta", "Terca" },
                values: new object[] { 10000f, 10000f, 10000f, 10000f, 10000f, 10000f, 10000f });

            migrationBuilder.CreateIndex(
                name: "IX_ItemPedidos_PedidoId",
                table: "ItemPedidos",
                column: "PedidoId");

            migrationBuilder.CreateIndex(
                name: "IX_Transferencias_CheckinId",
                table: "Transferencias",
                column: "CheckinId");

            migrationBuilder.CreateIndex(
                name: "IX_Transferencias_HospedagemDestinoId",
                table: "Transferencias",
                column: "HospedagemDestinoId");

            migrationBuilder.CreateIndex(
                name: "IX_Transferencias_HospedagemOrigemId",
                table: "Transferencias",
                column: "HospedagemOrigemId");

            migrationBuilder.CreateIndex(
                name: "IX_Transferencias_MotivoTransferenciaId",
                table: "Transferencias",
                column: "MotivoTransferenciaId");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemPedidos_Pedidos_PedidoId",
                table: "ItemPedidos",
                column: "PedidoId",
                principalTable: "Pedidos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Pedidos_Hospedes_HospedeId",
                table: "Pedidos",
                column: "HospedeId",
                principalTable: "Hospedes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Pedidos_PonteDeVendas_PontoVendaId",
                table: "Pedidos",
                column: "PontoVendaId",
                principalTable: "PonteDeVendas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItemPedidos_Pedidos_PedidoId",
                table: "ItemPedidos");

            migrationBuilder.DropForeignKey(
                name: "FK_Pedidos_Hospedes_HospedeId",
                table: "Pedidos");

            migrationBuilder.DropForeignKey(
                name: "FK_Pedidos_PonteDeVendas_PontoVendaId",
                table: "Pedidos");

            migrationBuilder.DropTable(
                name: "Transferencias");

            migrationBuilder.DropTable(
                name: "MotivoTransferencias");

            migrationBuilder.DropIndex(
                name: "IX_ItemPedidos_PedidoId",
                table: "ItemPedidos");

            migrationBuilder.DropColumn(
                name: "DataFinalizacao",
                table: "Pedidos");

            migrationBuilder.DropColumn(
                name: "DataPedido",
                table: "Pedidos");

            migrationBuilder.DropColumn(
                name: "NumePedido",
                table: "Pedidos");

            migrationBuilder.DropColumn(
                name: "Categoria",
                table: "ItemPedidos");

            migrationBuilder.DropColumn(
                name: "NomeProduto",
                table: "ItemPedidos");

            migrationBuilder.DropColumn(
                name: "Observacao",
                table: "ItemPedidos");

            migrationBuilder.DropColumn(
                name: "PedidoId",
                table: "ItemPedidos");

            migrationBuilder.DropColumn(
                name: "PrecoUnitario",
                table: "ItemPedidos");

            migrationBuilder.RenameColumn(
                name: "PontoVendaId",
                table: "Pedidos",
                newName: "PonteDeVendasId");

            migrationBuilder.RenameColumn(
                name: "HospedeId",
                table: "Pedidos",
                newName: "HospedesId");

            migrationBuilder.RenameIndex(
                name: "IX_Pedidos_PontoVendaId",
                table: "Pedidos",
                newName: "IX_Pedidos_PonteDeVendasId");

            migrationBuilder.RenameIndex(
                name: "IX_Pedidos_HospedeId",
                table: "Pedidos",
                newName: "IX_Pedidos_HospedesId");

            migrationBuilder.RenameColumn(
                name: "ProdutoId",
                table: "ItemPedidos",
                newName: "IdProduto");

            migrationBuilder.AlterColumn<string>(
                name: "SituacaoPagamento",
                table: "Pedidos",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<float>(
                name: "Valor",
                table: "Pedidos",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<int>(
                name: "PedidosId",
                table: "ItemPedidos",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "Preco",
                table: "ItemPedidos",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DataFechamento",
                table: "Hospedagems",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "TipoApartamentos",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Domingo", "Quarta", "Quinta", "Sabado", "Segunda", "Sexta", "Terca" },
                values: new object[] { 0f, 0f, 0f, 0f, 0f, 0f, 0f });

            migrationBuilder.UpdateData(
                table: "TipoApartamentos",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Domingo", "Quarta", "Quinta", "Sabado", "Segunda", "Sexta", "Terca" },
                values: new object[] { 0f, 0f, 0f, 0f, 0f, 0f, 0f });

            migrationBuilder.UpdateData(
                table: "TipoApartamentos",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Domingo", "Quarta", "Quinta", "Sabado", "Segunda", "Sexta", "Terca" },
                values: new object[] { 0f, 0f, 0f, 0f, 0f, 0f, 0f });

            migrationBuilder.UpdateData(
                table: "TipoApartamentos",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Domingo", "Quarta", "Quinta", "Sabado", "Segunda", "Sexta", "Terca" },
                values: new object[] { 0f, 0f, 0f, 0f, 0f, 0f, 0f });

            migrationBuilder.CreateIndex(
                name: "IX_ItemPedidos_PedidosId",
                table: "ItemPedidos",
                column: "PedidosId");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemPedidos_Pedidos_PedidosId",
                table: "ItemPedidos",
                column: "PedidosId",
                principalTable: "Pedidos",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Pedidos_Hospedes_HospedesId",
                table: "Pedidos",
                column: "HospedesId",
                principalTable: "Hospedes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Pedidos_PonteDeVendas_PonteDeVendasId",
                table: "Pedidos",
                column: "PonteDeVendasId",
                principalTable: "PonteDeVendas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
