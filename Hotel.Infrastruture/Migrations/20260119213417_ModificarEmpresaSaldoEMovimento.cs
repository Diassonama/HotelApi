using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hotel.Infrastruture.Migrations
{
    /// <inheritdoc />
    public partial class ModificarEmpresaSaldoEMovimento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Produtos_PlanoDeContas_PlanoDeContaId",
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

            migrationBuilder.DropIndex(
                name: "IX_Produtos_PlanoDeContaId",
                table: "Produtos");

            migrationBuilder.DropIndex(
                name: "IX_Produtos_TipoProdutoId",
                table: "Produtos");

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

            migrationBuilder.DropColumn(
                name: "PlanoDeContaId",
                table: "Produtos");

            migrationBuilder.DropColumn(
                name: "TipoProdutoId",
                table: "Produtos");

            migrationBuilder.AddColumn<int>(
                name: "ProdutoId1",
                table: "ProdutoStocks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<float>(
                name: "ValorFixo",
                table: "Produtos",
                type: "real",
                nullable: false,
                defaultValue: 0f,
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
                defaultValue: 1,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "TaxExemptionReasonCode",
                table: "Produtos",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "M00",
                oldClrType: typeof(string),
                oldType: "nvarchar(3)",
                oldMaxLength: 3,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ProductTypeCode",
                table: "Produtos",
                type: "nvarchar(1)",
                maxLength: 1,
                nullable: false,
                defaultValue: "P",
                oldClrType: typeof(string),
                oldType: "nvarchar(1)",
                oldMaxLength: 1,
                oldNullable: true);

            migrationBuilder.AlterColumn<float>(
                name: "PrecoCompra",
                table: "Produtos",
                type: "real",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<float>(
                name: "PrecoCIva",
                table: "Produtos",
                type: "real",
                nullable: false,
                defaultValue: 0f,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldDefaultValue: 0m);

            migrationBuilder.AlterColumn<float>(
                name: "MargemLucro",
                table: "Produtos",
                type: "real",
                nullable: false,
                defaultValue: 0f,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldDefaultValue: 0m);

            migrationBuilder.AlterColumn<float>(
                name: "Lucro",
                table: "Produtos",
                type: "real",
                nullable: false,
                defaultValue: 0f,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldDefaultValue: 0m);

            migrationBuilder.AlterColumn<float>(
                name: "DescontoPercentagem",
                table: "Produtos",
                type: "real",
                nullable: false,
                defaultValue: 0f,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldDefaultValue: 0m);

            migrationBuilder.AlterColumn<float>(
                name: "Desconto",
                table: "Produtos",
                type: "real",
                nullable: false,
                defaultValue: 0f,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldDefaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "EmpresaSaldos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmpresaId = table.Column<int>(type: "int", nullable: false),
                    Saldo = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IdTenant = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmpresaSaldos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmpresaSaldos_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OnlineStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    IsOnline = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    LastSeen = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ConnectionId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IdTenant = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OnlineStatuses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OnlineStatuses_User",
                        column: x => x.UserId,
                        principalTable: "Utilizador",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Produto",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Valor = table.Column<float>(type: "real", nullable: false),
                    Quantidade = table.Column<int>(type: "int", nullable: false),
                    EstoqueMinimo = table.Column<int>(type: "int", nullable: false),
                    AdicionarStock = table.Column<int>(type: "int", nullable: false),
                    Lucro = table.Column<float>(type: "real", nullable: false),
                    MargemLucro = table.Column<float>(type: "real", nullable: false),
                    DataExpiracao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ValorFixo = table.Column<float>(type: "real", nullable: false),
                    PrecoCompra = table.Column<float>(type: "real", nullable: false),
                    Desconto = table.Column<float>(type: "real", nullable: false),
                    DescontoPercentagem = table.Column<float>(type: "real", nullable: false),
                    PontoDeVendasId = table.Column<int>(type: "int", nullable: false),
                    CaminhoImagem = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CategoriaId = table.Column<int>(type: "int", nullable: false),
                    PrecoCIva = table.Column<float>(type: "real", nullable: false),
                    ProductTypeCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TaxTableEntryId = table.Column<int>(type: "int", nullable: false),
                    TaxExemptionReasonCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PontoDeVendaId = table.Column<int>(type: "int", nullable: true),
                    ProductTypesProductTypeCode = table.Column<string>(type: "nvarchar(1)", nullable: true),
                    TaxExemptionReasonTaxExemptionCode = table.Column<string>(type: "nvarchar(3)", nullable: true),
                    PlanoDeContaId = table.Column<int>(type: "int", nullable: true),
                    TipoProdutoId = table.Column<int>(type: "int", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IdTenant = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Produto", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Produto_Categorias_CategoriaId",
                        column: x => x.CategoriaId,
                        principalTable: "Categorias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Produto_PlanoDeContas_PlanoDeContaId",
                        column: x => x.PlanoDeContaId,
                        principalTable: "PlanoDeContas",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Produto_PonteDeVendas_PontoDeVendaId",
                        column: x => x.PontoDeVendaId,
                        principalTable: "PonteDeVendas",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Produto_ProductTypes_ProductTypesProductTypeCode",
                        column: x => x.ProductTypesProductTypeCode,
                        principalTable: "ProductTypes",
                        principalColumn: "ProductTypeCode");
                    table.ForeignKey(
                        name: "FK_Produto_TaxExemptionReasons_TaxExemptionReasonTaxExemptionCode",
                        column: x => x.TaxExemptionReasonTaxExemptionCode,
                        principalTable: "TaxExemptionReasons",
                        principalColumn: "TaxExemptionCode");
                    table.ForeignKey(
                        name: "FK_Produto_TaxTableEntry_TaxTableEntryId",
                        column: x => x.TaxTableEntryId,
                        principalTable: "TaxTableEntry",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Produto_TipoProdutos_TipoProdutoId",
                        column: x => x.TipoProdutoId,
                        principalTable: "TipoProdutos",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Transferencias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CheckinId = table.Column<int>(type: "int", nullable: false),
                    MotivoTransferenciaId = table.Column<int>(type: "int", nullable: false),
                    QuartoId = table.Column<int>(type: "int", nullable: false),
                    TipoTransferencia = table.Column<int>(type: "int", nullable: false),
                    ManterPreco = table.Column<bool>(type: "bit", nullable: false),
                    ValorDiaria = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DataEntrada = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataSaida = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataTransferencia = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Observacao = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    UtilizadorId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    MotivoTransferenciaId1 = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transferencias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transferencias_Apartamentos_QuartoId",
                        column: x => x.QuartoId,
                        principalTable: "Apartamentos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Transferencias_Checkins_CheckinId",
                        column: x => x.CheckinId,
                        principalTable: "Checkins",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Transferencias_MotivoTransferencias_MotivoTransferenciaId",
                        column: x => x.MotivoTransferenciaId,
                        principalTable: "MotivoTransferencias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Transferencias_MotivoTransferencias_MotivoTransferenciaId1",
                        column: x => x.MotivoTransferenciaId1,
                        principalTable: "MotivoTransferencias",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Transferencias_Utilizador_UtilizadorId",
                        column: x => x.UtilizadorId,
                        principalTable: "Utilizador",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmpresaSaldoMovimentos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmpresaSaldoId = table.Column<int>(type: "int", nullable: false),
                    Valor = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UtilizadorId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Documento = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Observacao = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TipoLancamento = table.Column<int>(type: "int", nullable: false),
                    UtilizadorId1 = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IdTenant = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmpresaSaldoMovimentos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmpresaSaldoMovimentos_EmpresaSaldos_EmpresaSaldoId",
                        column: x => x.EmpresaSaldoId,
                        principalTable: "EmpresaSaldos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmpresaSaldoMovimentos_Utilizador_UtilizadorId",
                        column: x => x.UtilizadorId,
                        principalTable: "Utilizador",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmpresaSaldoMovimentos_Utilizador_UtilizadorId1",
                        column: x => x.UtilizadorId1,
                        principalTable: "Utilizador",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ConversationParticipants",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ConversationId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    JoinedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IdTenant = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConversationParticipants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConversationParticipants_User",
                        column: x => x.UserId,
                        principalTable: "Utilizador",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Conversations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ConversationId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UnreadCount = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastMessageId = table.Column<int>(type: "int", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IdTenant = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Conversations", x => x.Id);
                    table.UniqueConstraint("AK_Conversations_ConversationId", x => x.ConversationId);
                });

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SenderId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ReceiverId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Content = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    MessageType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AttachmentUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ConversationId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IdTenant = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Messages_Conversation",
                        column: x => x.ConversationId,
                        principalTable: "Conversations",
                        principalColumn: "ConversationId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Messages_Receiver",
                        column: x => x.ReceiverId,
                        principalTable: "Utilizador",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Messages_Sender",
                        column: x => x.SenderId,
                        principalTable: "Utilizador",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MessageNotifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MessageId = table.Column<int>(type: "int", nullable: false),
                    SenderId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    ReceiverId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    IsNew = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IdTenant = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageNotifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MessageNotifications_Message",
                        column: x => x.MessageId,
                        principalTable: "Messages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MessageNotifications_Receiver",
                        column: x => x.ReceiverId,
                        principalTable: "Utilizador",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MessageNotifications_Sender",
                        column: x => x.SenderId,
                        principalTable: "Utilizador",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_ProdutoStocks_ProdutoId1",
                table: "ProdutoStocks",
                column: "ProdutoId1");

            migrationBuilder.CreateIndex(
                name: "IX_ConversationParticipants_ConversationUser",
                table: "ConversationParticipants",
                columns: new[] { "ConversationId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ConversationParticipants_UserId",
                table: "ConversationParticipants",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Conversations_ConversationId",
                table: "Conversations",
                column: "ConversationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Conversations_LastMessageId",
                table: "Conversations",
                column: "LastMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_Conversations_UpdatedAt",
                table: "Conversations",
                column: "UpdatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_EmpresaSaldoMovimentos_EmpresaSaldoId",
                table: "EmpresaSaldoMovimentos",
                column: "EmpresaSaldoId");

            migrationBuilder.CreateIndex(
                name: "IX_EmpresaSaldoMovimentos_UtilizadorId",
                table: "EmpresaSaldoMovimentos",
                column: "UtilizadorId");

            migrationBuilder.CreateIndex(
                name: "IX_EmpresaSaldoMovimentos_UtilizadorId1",
                table: "EmpresaSaldoMovimentos",
                column: "UtilizadorId1");

            migrationBuilder.CreateIndex(
                name: "IX_EmpresaSaldos_EmpresaId",
                table: "EmpresaSaldos",
                column: "EmpresaId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MessageNotifications_MessageId",
                table: "MessageNotifications",
                column: "MessageId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageNotifications_ReceiverId",
                table: "MessageNotifications",
                column: "ReceiverId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageNotifications_ReceiverId_IsRead",
                table: "MessageNotifications",
                columns: new[] { "ReceiverId", "IsRead" });

            migrationBuilder.CreateIndex(
                name: "IX_MessageNotifications_SenderId",
                table: "MessageNotifications",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageNotifications_Timestamp",
                table: "MessageNotifications",
                column: "Timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ConversationId",
                table: "Messages",
                column: "ConversationId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ReceiverId_IsRead",
                table: "Messages",
                columns: new[] { "ReceiverId", "IsRead" });

            migrationBuilder.CreateIndex(
                name: "IX_Messages_SenderId_ReceiverId",
                table: "Messages",
                columns: new[] { "SenderId", "ReceiverId" });

            migrationBuilder.CreateIndex(
                name: "IX_Messages_Timestamp",
                table: "Messages",
                column: "Timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_OnlineStatuses_IsOnline",
                table: "OnlineStatuses",
                column: "IsOnline");

            migrationBuilder.CreateIndex(
                name: "IX_OnlineStatuses_UserId",
                table: "OnlineStatuses",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Produto_CategoriaId",
                table: "Produto",
                column: "CategoriaId");

            migrationBuilder.CreateIndex(
                name: "IX_Produto_PlanoDeContaId",
                table: "Produto",
                column: "PlanoDeContaId");

            migrationBuilder.CreateIndex(
                name: "IX_Produto_PontoDeVendaId",
                table: "Produto",
                column: "PontoDeVendaId");

            migrationBuilder.CreateIndex(
                name: "IX_Produto_ProductTypesProductTypeCode",
                table: "Produto",
                column: "ProductTypesProductTypeCode");

            migrationBuilder.CreateIndex(
                name: "IX_Produto_TaxExemptionReasonTaxExemptionCode",
                table: "Produto",
                column: "TaxExemptionReasonTaxExemptionCode");

            migrationBuilder.CreateIndex(
                name: "IX_Produto_TaxTableEntryId",
                table: "Produto",
                column: "TaxTableEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_Produto_TipoProdutoId",
                table: "Produto",
                column: "TipoProdutoId");

            migrationBuilder.CreateIndex(
                name: "IX_Transferencias_CheckinId",
                table: "Transferencias",
                column: "CheckinId");

            migrationBuilder.CreateIndex(
                name: "IX_Transferencias_MotivoTransferenciaId",
                table: "Transferencias",
                column: "MotivoTransferenciaId");

            migrationBuilder.CreateIndex(
                name: "IX_Transferencias_MotivoTransferenciaId1",
                table: "Transferencias",
                column: "MotivoTransferenciaId1");

            migrationBuilder.CreateIndex(
                name: "IX_Transferencias_QuartoId",
                table: "Transferencias",
                column: "QuartoId");

            migrationBuilder.CreateIndex(
                name: "IX_Transferencias_UtilizadorId",
                table: "Transferencias",
                column: "UtilizadorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Produtos_ProductTypes_ProductTypeCode",
                table: "Produtos",
                column: "ProductTypeCode",
                principalTable: "ProductTypes",
                principalColumn: "ProductTypeCode",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Produtos_TaxExemptionReasons_TaxExemptionReasonCode",
                table: "Produtos",
                column: "TaxExemptionReasonCode",
                principalTable: "TaxExemptionReasons",
                principalColumn: "TaxExemptionCode",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Produtos_TaxTableEntry_TaxTableEntryId",
                table: "Produtos",
                column: "TaxTableEntryId",
                principalTable: "TaxTableEntry",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProdutoStocks_Produto_ProdutoId1",
                table: "ProdutoStocks",
                column: "ProdutoId1",
                principalTable: "Produto",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ConversationParticipants_Conversation",
                table: "ConversationParticipants",
                column: "ConversationId",
                principalTable: "Conversations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Conversations_LastMessage",
                table: "Conversations",
                column: "LastMessageId",
                principalTable: "Messages",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
                name: "FK_ProdutoStocks_Produto_ProdutoId1",
                table: "ProdutoStocks");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Conversation",
                table: "Messages");

            migrationBuilder.DropTable(
                name: "ConversationParticipants");

            migrationBuilder.DropTable(
                name: "EmpresaSaldoMovimentos");

            migrationBuilder.DropTable(
                name: "MessageNotifications");

            migrationBuilder.DropTable(
                name: "OnlineStatuses");

            migrationBuilder.DropTable(
                name: "Produto");

            migrationBuilder.DropTable(
                name: "Transferencias");

            migrationBuilder.DropTable(
                name: "EmpresaSaldos");

            migrationBuilder.DropTable(
                name: "Conversations");

            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_ProdutoStocks_ProdutoId1",
                table: "ProdutoStocks");

            migrationBuilder.DropColumn(
                name: "ProdutoId1",
                table: "ProdutoStocks");

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

            migrationBuilder.AlterColumn<decimal>(
                name: "ValorFixo",
                table: "Produtos",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(float),
                oldType: "real",
                oldDefaultValue: 0f);

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
                oldType: "int",
                oldDefaultValue: 1);

            migrationBuilder.AlterColumn<string>(
                name: "TaxExemptionReasonCode",
                table: "Produtos",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(3)",
                oldMaxLength: 3,
                oldDefaultValue: "M00");

            migrationBuilder.AlterColumn<string>(
                name: "ProductTypeCode",
                table: "Produtos",
                type: "nvarchar(1)",
                maxLength: 1,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1)",
                oldMaxLength: 1,
                oldDefaultValue: "P");

            migrationBuilder.AlterColumn<decimal>(
                name: "PrecoCompra",
                table: "Produtos",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AlterColumn<decimal>(
                name: "PrecoCIva",
                table: "Produtos",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(float),
                oldType: "real",
                oldDefaultValue: 0f);

            migrationBuilder.AlterColumn<decimal>(
                name: "MargemLucro",
                table: "Produtos",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(float),
                oldType: "real",
                oldDefaultValue: 0f);

            migrationBuilder.AlterColumn<decimal>(
                name: "Lucro",
                table: "Produtos",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(float),
                oldType: "real",
                oldDefaultValue: 0f);

            migrationBuilder.AlterColumn<decimal>(
                name: "DescontoPercentagem",
                table: "Produtos",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(float),
                oldType: "real",
                oldDefaultValue: 0f);

            migrationBuilder.AlterColumn<decimal>(
                name: "Desconto",
                table: "Produtos",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(float),
                oldType: "real",
                oldDefaultValue: 0f);

            migrationBuilder.AddColumn<int>(
                name: "PlanoDeContaId",
                table: "Produtos",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TipoProdutoId",
                table: "Produtos",
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

            migrationBuilder.CreateIndex(
                name: "IX_Produtos_PlanoDeContaId",
                table: "Produtos",
                column: "PlanoDeContaId");

            migrationBuilder.CreateIndex(
                name: "IX_Produtos_TipoProdutoId",
                table: "Produtos",
                column: "TipoProdutoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Produtos_PlanoDeContas_PlanoDeContaId",
                table: "Produtos",
                column: "PlanoDeContaId",
                principalTable: "PlanoDeContas",
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
