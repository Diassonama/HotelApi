using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Hotel.Infrastruture.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppConfig",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Key = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppConfig", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppMenu",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PreIcon = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PostIcon = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Path = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IdTenant = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppMenu", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Contas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Descricao = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IdTenant = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DocumentosVendas",
                columns: table => new
                {
                    Documento = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Diario = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PagarReceber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TipoConta = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Estado = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentosVendas", x => x.Documento);
                });

            migrationBuilder.CreateTable(
                name: "Empresas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RazaoSocial = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Telefone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PercentualDesconto = table.Column<float>(type: "real", nullable: false),
                    Endereco = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Bairro = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Cidade = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NumContribuinte = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IdTenant = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Empresas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MenuItem",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Perfil = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Label = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParentId = table.Column<int>(type: "int", nullable: true),
                    SubmenuRef = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MenuItem_MenuItem_ParentId",
                        column: x => x.ParentId,
                        principalTable: "MenuItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MobiliaTipoApartamentos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IdTenant = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MobiliaTipoApartamentos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MotivoViagens",
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
                    table.PrimaryKey("PK_MotivoViagens", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Paises",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Nacionalidade = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IdTenant = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Paises", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Params",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Taxa = table.Column<int>(type: "int", nullable: false),
                    CalcularTaxa = table.Column<int>(type: "int", nullable: false),
                    CalcularHora = table.Column<int>(type: "int", nullable: false),
                    Tolerancia = table.Column<int>(type: "int", nullable: false),
                    Regime = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SistemaContabilistico = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DataInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataFim = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Estabelecimento = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Isencao = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IVA = table.Column<int>(type: "int", nullable: false),
                    RegistroPorPagina = table.Column<int>(type: "int", nullable: false),
                    TipoRecibo = table.Column<int>(type: "int", nullable: false),
                    NomeEmpresa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Endereco = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Cidade = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NumContribuinte = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Telefone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LogoCaminho = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContaBancaria = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IdTenant = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Params", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Patrimonios",
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
                    table.PrimaryKey("PK_Patrimonios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Perfis",
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
                    table.PrimaryKey("PK_Perfis", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PonteDeVendas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getdate()"),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IdTenant = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PonteDeVendas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductTypes",
                columns: table => new
                {
                    ProductTypeCode = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    ProductTypeDescription = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductTypes", x => x.ProductTypeCode);
                });

            migrationBuilder.CreateTable(
                name: "RefreshToken",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Token = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Expires = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByIp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Revoked = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RevokedByIp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReplacedByToken = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshToken", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Serial",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NumSerial = table.Column<int>(type: "int", nullable: false),
                    Chave = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DataInicial = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContadorData = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UltimoAcesso = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Prazo = table.Column<int>(type: "int", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IdTenant = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Serial", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Series",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TipoDoc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Serie = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Numerador = table.Column<int>(type: "int", nullable: false),
                    DataUltimoDocumento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Ano = table.Column<int>(type: "int", nullable: false),
                    NumVias = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AutoFacturacao = table.Column<bool>(type: "bit", nullable: false),
                    Assinatura = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IdTenant = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Series", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TaxAccountingBasis",
                columns: table => new
                {
                    TaxAccounting = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AccountingDescription = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaxAccountingBasis", x => x.TaxAccounting);
                });

            migrationBuilder.CreateTable(
                name: "TaxTypes",
                columns: table => new
                {
                    TaxType = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaxTypes", x => x.TaxType);
                });

            migrationBuilder.CreateTable(
                name: "TipoGovernancas",
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
                    table.PrimaryKey("PK_TipoGovernancas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TipoHospedagens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Valor = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IdTenant = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipoHospedagens", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TipoPagamentos",
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
                    table.PrimaryKey("PK_TipoPagamentos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TipoProdutos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IdTenant = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipoProdutos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TipoRecibos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RPT = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IdTenant = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipoRecibos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PlanoDeContas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Descricao = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    ContasId = table.Column<int>(type: "int", nullable: false),
                    ContaId = table.Column<int>(type: "int", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IdTenant = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanoDeContas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlanoDeContas_Contas_ContaId",
                        column: x => x.ContaId,
                        principalTable: "Contas",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PlanoDeContas_Contas_ContasId",
                        column: x => x.ContasId,
                        principalTable: "Contas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TipoApartamentos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ValorDiariaSingle = table.Column<float>(type: "real", nullable: false),
                    ValorDiariaDouble = table.Column<float>(type: "real", nullable: false),
                    ValorDiariaTriple = table.Column<float>(type: "real", nullable: false),
                    ValorDiariaQuadruple = table.Column<float>(type: "real", nullable: false),
                    ValorUmaHora = table.Column<float>(type: "real", nullable: false),
                    ValorDuasHora = table.Column<float>(type: "real", nullable: false),
                    ValorTresHora = table.Column<float>(type: "real", nullable: false),
                    ValorQuatroHora = table.Column<float>(type: "real", nullable: false),
                    ValorNoite = table.Column<float>(type: "real", nullable: false),
                    Segunda = table.Column<float>(type: "real", nullable: false),
                    Terca = table.Column<float>(type: "real", nullable: false),
                    Quarta = table.Column<float>(type: "real", nullable: false),
                    Quinta = table.Column<float>(type: "real", nullable: false),
                    Sexta = table.Column<float>(type: "real", nullable: false),
                    Sabado = table.Column<float>(type: "real", nullable: false),
                    Domingo = table.Column<float>(type: "real", nullable: false),
                    MobiliaTipoApartamentoId = table.Column<int>(type: "int", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IdTenant = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipoApartamentos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TipoApartamentos_MobiliaTipoApartamentos_MobiliaTipoApartamentoId",
                        column: x => x.MobiliaTipoApartamentoId,
                        principalTable: "MobiliaTipoApartamentos",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Clientes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Telefone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Celular = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DataAniversario = table.Column<DateTime>(type: "Date", nullable: false),
                    Generos = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Observacao = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmpresasId = table.Column<int>(type: "int", nullable: false),
                    Profissao = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PercetualDesconto = table.Column<int>(type: "int", nullable: false),
                    PaisId = table.Column<int>(type: "int", nullable: false),
                    Endereco = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Bairro = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Cidade = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Complemento = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Preferencias = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Idioma = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HoraParaAcordar = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HoraParaDormir = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Cargo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IdTenant = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clientes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Clientes_Empresas_EmpresasId",
                        column: x => x.EmpresasId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Clientes_Paises_PaisId",
                        column: x => x.PaisId,
                        principalTable: "Paises",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MobiliaTipoApartamentoPatrimonio",
                columns: table => new
                {
                    MobiliaTipoApartamentosId = table.Column<int>(type: "int", nullable: false),
                    patrimoniosId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MobiliaTipoApartamentoPatrimonio", x => new { x.MobiliaTipoApartamentosId, x.patrimoniosId });
                    table.ForeignKey(
                        name: "FK_MobiliaTipoApartamentoPatrimonio_MobiliaTipoApartamentos_MobiliaTipoApartamentosId",
                        column: x => x.MobiliaTipoApartamentosId,
                        principalTable: "MobiliaTipoApartamentos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MobiliaTipoApartamentoPatrimonio_Patrimonios_patrimoniosId",
                        column: x => x.patrimoniosId,
                        principalTable: "Patrimonios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Acessos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PreIcon = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PostIcon = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Path = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PerfilsId = table.Column<int>(type: "int", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IdTenant = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Acessos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Acessos_Perfis_PerfilsId",
                        column: x => x.PerfilsId,
                        principalTable: "Perfis",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Utilizador",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RefreshTokenId = table.Column<int>(type: "int", nullable: true),
                    Discriminator = table.Column<string>(type: "nvarchar(21)", maxLength: 21, nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Utilizador", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Utilizador_RefreshToken_RefreshTokenId",
                        column: x => x.RefreshTokenId,
                        principalTable: "RefreshToken",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MenuRole",
                columns: table => new
                {
                    MenuId = table.Column<int>(type: "int", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuRole", x => new { x.MenuId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_MenuRole_AppMenu_MenuId",
                        column: x => x.MenuId,
                        principalTable: "AppMenu",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MenuRole_Role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoleClaims_Role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tax",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TaxCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: true),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TaxType = table.Column<string>(type: "nvarchar(3)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tax", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tax_TaxTypes_TaxType",
                        column: x => x.TaxType,
                        principalTable: "TaxTypes",
                        principalColumn: "TaxType");
                });

            migrationBuilder.CreateTable(
                name: "TaxTableEntry",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TaxCountryRegion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TaxCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TaxType = table.Column<string>(type: "nvarchar(3)", nullable: true),
                    TaxExpirationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TaxPercentage = table.Column<float>(type: "real", nullable: false),
                    TaxAmount = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaxTableEntry", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaxTableEntry_TaxTypes_TaxType",
                        column: x => x.TaxType,
                        principalTable: "TaxTypes",
                        principalColumn: "TaxType");
                });

            migrationBuilder.CreateTable(
                name: "Caixas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SaldoInicial = table.Column<float>(type: "real", nullable: false),
                    SaldoFinal = table.Column<float>(type: "real", nullable: false),
                    SaldoAtual = table.Column<float>(type: "real", nullable: false),
                    SaldoPendenteCaixaAnterior = table.Column<float>(type: "real", nullable: false),
                    SaldoPendeteCaixaAtual = table.Column<float>(type: "real", nullable: false),
                    DataDeAbertura = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataDeFechamento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Entrada = table.Column<float>(type: "real", nullable: false),
                    Saida = table.Column<float>(type: "real", nullable: false),
                    UtilizadoresId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IdTenant = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Caixas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Caixas_Utilizador_UtilizadoresId",
                        column: x => x.UtilizadoresId,
                        principalTable: "Utilizador",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "lavandarias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CaixaAberto = table.Column<int>(type: "int", nullable: false),
                    DataEntrada = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataSaida = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Valor = table.Column<float>(type: "real", nullable: false),
                    SituacaoPagamento = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RegistoPagamento = table.Column<int>(type: "int", nullable: false),
                    UtilizadoresId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClientesId = table.Column<int>(type: "int", nullable: false),
                    PercentagemDesconto = table.Column<float>(type: "real", nullable: false),
                    ValorDesconto = table.Column<float>(type: "real", nullable: false),
                    SituacaoServico = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IdTenant = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_lavandarias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_lavandarias_Clientes_ClientesId",
                        column: x => x.ClientesId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_lavandarias_Utilizador_UtilizadoresId",
                        column: x => x.UtilizadoresId,
                        principalTable: "Utilizador",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reservas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmpresaId = table.Column<int>(type: "int", nullable: false),
                    NPX = table.Column<int>(type: "int", nullable: false),
                    QuantidadeQuartos = table.Column<int>(type: "int", nullable: false),
                    TotalGeral = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UtilizadoresId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IdTenant = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reservas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reservas_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Reservas_Utilizador_UtilizadoresId",
                        column: x => x.UtilizadoresId,
                        principalTable: "Utilizador",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UtilizadorClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UtilizadorClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UtilizadorClaims_Utilizador_UserId",
                        column: x => x.UserId,
                        principalTable: "Utilizador",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UtilizadorLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UtilizadorLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_UtilizadorLogins_Utilizador_UserId",
                        column: x => x.UserId,
                        principalTable: "Utilizador",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UtilizadorRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UtilizadorRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_UtilizadorRoles_Role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UtilizadorRoles_Utilizador_UserId",
                        column: x => x.UserId,
                        principalTable: "Utilizador",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UtilizadorTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UtilizadorTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_UtilizadorTokens_Utilizador_UserId",
                        column: x => x.UserId,
                        principalTable: "Utilizador",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TaxExemptionReasons",
                columns: table => new
                {
                    TaxExemptionCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    TaxExemptionReasons = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TaxCode = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaxExemptionReasons", x => x.TaxExemptionCode);
                    table.ForeignKey(
                        name: "FK_TaxExemptionReasons_TaxTableEntry_TaxCode",
                        column: x => x.TaxCode,
                        principalTable: "TaxTableEntry",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Checkins",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdCaixaCheckin = table.Column<int>(type: "int", nullable: false),
                    IdCaixaCheckOut = table.Column<int>(type: "int", nullable: false),
                    DataSaida = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DataEntrada = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdHospedeCheckOut = table.Column<int>(type: "int", nullable: false),
                    ValorTotalDiaria = table.Column<float>(type: "real", nullable: false),
                    ValorTotalConsumo = table.Column<float>(type: "real", nullable: false),
                    ValorTotalLigacao = table.Column<float>(type: "real", nullable: false),
                    ValorTotalFinal = table.Column<float>(type: "real", nullable: false),
                    ValorDesconto = table.Column<float>(type: "real", nullable: false),
                    IdUtilizadorCheckin = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    IdUtilizadorCheckOut = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CheckoutRealizado = table.Column<bool>(type: "bit", nullable: false),
                    PercentualDesconto = table.Column<float>(type: "real", nullable: false),
                    Observacao = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CamaExtra = table.Column<int>(type: "int", nullable: false),
                    situacaoDoPagamento = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CaixaId = table.Column<int>(type: "int", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IdTenant = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Checkins", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Checkins_Caixas_CaixaId",
                        column: x => x.CaixaId,
                        principalTable: "Caixas",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Checkins_Caixas_IdCaixaCheckOut",
                        column: x => x.IdCaixaCheckOut,
                        principalTable: "Caixas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Checkins_Caixas_IdCaixaCheckin",
                        column: x => x.IdCaixaCheckin,
                        principalTable: "Caixas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Checkins_Utilizador_IdUtilizadorCheckOut",
                        column: x => x.IdUtilizadorCheckOut,
                        principalTable: "Utilizador",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Checkins_Utilizador_IdUtilizadorCheckin",
                        column: x => x.IdUtilizadorCheckin,
                        principalTable: "Utilizador",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "lavandariaItens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Quantidade = table.Column<int>(type: "int", nullable: false),
                    Preco = table.Column<float>(type: "real", nullable: false),
                    Cor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Tamanho = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Observacao = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DataEntrega = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Entregue = table.Column<bool>(type: "bit", nullable: false),
                    LavandariasId = table.Column<int>(type: "int", nullable: true),
                    UtilizadoresId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IdTenant = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_lavandariaItens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_lavandariaItens_Utilizador_UtilizadoresId",
                        column: x => x.UtilizadoresId,
                        principalTable: "Utilizador",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_lavandariaItens_lavandarias_LavandariasId",
                        column: x => x.LavandariasId,
                        principalTable: "lavandarias",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Produtos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Valor = table.Column<float>(type: "real", nullable: false),
                    Quantidade = table.Column<int>(type: "int", nullable: false),
                    EstoqueMinino = table.Column<int>(type: "int", nullable: false),
                    AdicionarStock = table.Column<int>(type: "int", nullable: false),
                    Lucro = table.Column<float>(type: "real", nullable: false),
                    MargemLucro = table.Column<float>(type: "real", nullable: false),
                    DataExpiracao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ValorFixo = table.Column<float>(type: "real", nullable: false),
                    PrecoCompra = table.Column<float>(type: "real", nullable: false),
                    Desconto = table.Column<float>(type: "real", nullable: false),
                    DescontoPercentagem = table.Column<float>(type: "real", nullable: false),
                    PlanoDeContasId = table.Column<int>(type: "int", nullable: true),
                    PrecoCIva = table.Column<float>(type: "real", nullable: false),
                    TipoProdutosId = table.Column<int>(type: "int", nullable: true),
                    ProductTypesProductTypeCode = table.Column<string>(type: "nvarchar(1)", nullable: true),
                    TaxTableEntryId = table.Column<int>(type: "int", nullable: true),
                    TaxExemptionReasonTaxExemptionCode = table.Column<string>(type: "nvarchar(3)", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IdTenant = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Produtos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Produtos_PlanoDeContas_PlanoDeContasId",
                        column: x => x.PlanoDeContasId,
                        principalTable: "PlanoDeContas",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Produtos_ProductTypes_ProductTypesProductTypeCode",
                        column: x => x.ProductTypesProductTypeCode,
                        principalTable: "ProductTypes",
                        principalColumn: "ProductTypeCode");
                    table.ForeignKey(
                        name: "FK_Produtos_TaxExemptionReasons_TaxExemptionReasonTaxExemptionCode",
                        column: x => x.TaxExemptionReasonTaxExemptionCode,
                        principalTable: "TaxExemptionReasons",
                        principalColumn: "TaxExemptionCode");
                    table.ForeignKey(
                        name: "FK_Produtos_TaxTableEntry_TaxTableEntryId",
                        column: x => x.TaxTableEntryId,
                        principalTable: "TaxTableEntry",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Produtos_TipoProdutos_TipoProdutosId",
                        column: x => x.TipoProdutosId,
                        principalTable: "TipoProdutos",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Apartamentos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    Observacao = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CodigoRamal = table.Column<int>(type: "int", nullable: false),
                    CafeDaManha = table.Column<int>(type: "int", nullable: false),
                    NaoPertube = table.Column<bool>(type: "bit", nullable: false),
                    Frigobar = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: true),
                    TipoGovernancasId = table.Column<int>(type: "int", nullable: false),
                    TipoApartamentosId = table.Column<int>(type: "int", nullable: false),
                    CheckinsId = table.Column<int>(type: "int", nullable: true),
                    Situacao = table.Column<string>(type: "varchar(20)", nullable: false, defaultValue: "Livre"),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IdTenant = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Apartamentos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Apartamentos_Checkins_CheckinsId",
                        column: x => x.CheckinsId,
                        principalTable: "Checkins",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Apartamentos_TipoApartamentos_TipoApartamentosId",
                        column: x => x.TipoApartamentosId,
                        principalTable: "TipoApartamentos",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Apartamentos_TipoGovernancas_TipoGovernancasId",
                        column: x => x.TipoGovernancasId,
                        principalTable: "TipoGovernancas",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Despertadores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NomeHospede = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DataHoraDespertar = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CheckinsId = table.Column<int>(type: "int", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IdTenant = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Despertadores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Despertadores_Checkins_CheckinsId",
                        column: x => x.CheckinsId,
                        principalTable: "Checkins",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "FacturaEmpresas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmpresasId = table.Column<int>(type: "int", nullable: false),
                    NumeroFactura = table.Column<int>(type: "int", nullable: false),
                    CheckinsId = table.Column<int>(type: "int", nullable: false),
                    Valor = table.Column<float>(type: "real", nullable: false),
                    Data = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Ano = table.Column<int>(type: "int", nullable: false),
                    Tipo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SituacaoFacturas = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IdTenant = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FacturaEmpresas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FacturaEmpresas_Checkins_CheckinsId",
                        column: x => x.CheckinsId,
                        principalTable: "Checkins",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FacturaEmpresas_Empresas_EmpresasId",
                        column: x => x.EmpresasId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Hospedes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientesId = table.Column<int>(type: "int", nullable: false),
                    CheckinsId = table.Column<int>(type: "int", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IdTenant = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hospedes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Hospedes_Checkins_CheckinsId",
                        column: x => x.CheckinsId,
                        principalTable: "Checkins",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Hospedes_Clientes_ClientesId",
                        column: x => x.ClientesId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProdutoStocks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Quantidade = table.Column<int>(type: "int", nullable: false),
                    ProdutoId = table.Column<int>(type: "int", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IdTenant = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProdutoStocks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProdutoStocks_Produtos_ProdutoId",
                        column: x => x.ProdutoId,
                        principalTable: "Produtos",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ApartamentosReservados",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReservasId = table.Column<int>(type: "int", nullable: false),
                    ApartamentosId = table.Column<int>(type: "int", nullable: false),
                    DataEntrada = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataSaida = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ClientesId = table.Column<int>(type: "int", nullable: false),
                    TipoHospedagensId = table.Column<int>(type: "int", nullable: false),
                    UtilizadoresId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ValorDiaria = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    QuantidadeDeDias = table.Column<int>(type: "int", nullable: false),
                    ReservaConfirmada = table.Column<bool>(type: "bit", nullable: false),
                    ReservaNoShow = table.Column<bool>(type: "bit", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IdTenant = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApartamentosReservados", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApartamentosReservados_Apartamentos_ApartamentosId",
                        column: x => x.ApartamentosId,
                        principalTable: "Apartamentos",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ApartamentosReservados_Clientes_ClientesId",
                        column: x => x.ClientesId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ApartamentosReservados_Reservas_ReservasId",
                        column: x => x.ReservasId,
                        principalTable: "Reservas",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ApartamentosReservados_TipoHospedagens_TipoHospedagensId",
                        column: x => x.TipoHospedagensId,
                        principalTable: "TipoHospedagens",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ApartamentosReservados_Utilizadores_UtilizadoresId",
                        column: x => x.UtilizadoresId,
                        principalTable: "Utilizador",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Governancas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApartamentosId = table.Column<int>(type: "int", nullable: false),
                    TipoGovernancasId = table.Column<int>(type: "int", nullable: false),
                    DataInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataTermino = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NomeDoResponsavel = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Observacao = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IdTenant = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Governancas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Governancas_Apartamentos_TipoGovernancasId",
                        column: x => x.TipoGovernancasId,
                        principalTable: "Apartamentos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Governancas_TipoGovernancas_TipoGovernancasId",
                        column: x => x.TipoGovernancasId,
                        principalTable: "TipoGovernancas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Hospedagems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DataAbertura = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataFechamento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PrevisaoFechamento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DiariaAntecipada = table.Column<bool>(type: "bit", nullable: false),
                    EarlyCheckin = table.Column<bool>(type: "bit", nullable: false),
                    QuantidadeCrianca = table.Column<int>(type: "int", nullable: false),
                    QuantidadeHomens = table.Column<int>(type: "int", nullable: false),
                    QuantidadeMulheres = table.Column<int>(type: "int", nullable: false),
                    QuantidadeDeDiarias = table.Column<int>(type: "int", nullable: false),
                    ValorDiaria = table.Column<float>(type: "real", nullable: false),
                    ApartamentosId = table.Column<int>(type: "int", nullable: false),
                    TipoHospedagensId = table.Column<int>(type: "int", nullable: false),
                    CheckinsId = table.Column<int>(type: "int", nullable: false),
                    EmpresasId = table.Column<int>(type: "int", nullable: false),
                    MotivoViagensId = table.Column<int>(type: "int", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IdTenant = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hospedagems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Hospedagems_Apartamentos_ApartamentosId",
                        column: x => x.ApartamentosId,
                        principalTable: "Apartamentos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Hospedagems_Checkins_CheckinsId",
                        column: x => x.CheckinsId,
                        principalTable: "Checkins",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Hospedagems_Empresas_EmpresasId",
                        column: x => x.EmpresasId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Hospedagems_MotivoViagens_MotivoViagensId",
                        column: x => x.MotivoViagensId,
                        principalTable: "MotivoViagens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Hospedagems_TipoHospedagens_TipoHospedagensId",
                        column: x => x.TipoHospedagensId,
                        principalTable: "TipoHospedagens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MobiliaApartamentos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Quantidade = table.Column<int>(type: "int", nullable: false),
                    PatrimoniosId = table.Column<int>(type: "int", nullable: true),
                    TipoApartamentosId = table.Column<int>(type: "int", nullable: true),
                    ApartamentosId = table.Column<int>(type: "int", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IdTenant = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MobiliaApartamentos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MobiliaApartamentos_Apartamentos_ApartamentosId",
                        column: x => x.ApartamentosId,
                        principalTable: "Apartamentos",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MobiliaApartamentos_Patrimonios_PatrimoniosId",
                        column: x => x.PatrimoniosId,
                        principalTable: "Patrimonios",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MobiliaApartamentos_TipoApartamentos_TipoApartamentosId",
                        column: x => x.TipoApartamentosId,
                        principalTable: "TipoApartamentos",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Pedidos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdCaixa = table.Column<int>(type: "int", nullable: false),
                    IdCheckin = table.Column<int>(type: "int", nullable: false),
                    Observacao = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Valor = table.Column<float>(type: "real", nullable: false),
                    SituacaoPagamento = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PonteDeVendasId = table.Column<int>(type: "int", nullable: false),
                    HospedesId = table.Column<int>(type: "int", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IdTenant = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pedidos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pedidos_Hospedes_HospedesId",
                        column: x => x.HospedesId,
                        principalTable: "Hospedes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Pedidos_PonteDeVendas_PonteDeVendasId",
                        column: x => x.PonteDeVendasId,
                        principalTable: "PonteDeVendas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FacturaDivididas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DataEntrada = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataSaida = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Valor = table.Column<float>(type: "real", nullable: false),
                    Grupo = table.Column<int>(type: "int", nullable: false),
                    IdHospedagem = table.Column<int>(type: "int", nullable: false),
                    DateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CheckinsId = table.Column<int>(type: "int", nullable: true),
                    HospedagensId = table.Column<int>(type: "int", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IdTenant = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FacturaDivididas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FacturaDivididas_Checkins_CheckinsId",
                        column: x => x.CheckinsId,
                        principalTable: "Checkins",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FacturaDivididas_Hospedagems_HospedagensId",
                        column: x => x.HospedagensId,
                        principalTable: "Hospedagems",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Historicos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CaixaAberto = table.Column<int>(type: "int", nullable: false),
                    DataHora = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Observacao = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UtilizadoresId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CheckinsId = table.Column<int>(type: "int", nullable: false),
                    HospedagemId = table.Column<int>(type: "int", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IdTenant = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Historicos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Historicos_Checkins_CheckinsId",
                        column: x => x.CheckinsId,
                        principalTable: "Checkins",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Historicos_Hospedagems_HospedagemId",
                        column: x => x.HospedagemId,
                        principalTable: "Hospedagems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Historicos_Utilizador_UtilizadoresId",
                        column: x => x.UtilizadoresId,
                        principalTable: "Utilizador",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Pagamentos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Valor = table.Column<float>(type: "real", nullable: false),
                    DataVencimento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Observacao = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IdVenda = table.Column<int>(type: "int", nullable: false),
                    HospedesId = table.Column<int>(type: "int", nullable: false),
                    CheckinsId = table.Column<int>(type: "int", nullable: false),
                    UtilizadoresId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    HospedagensId = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IdTenant = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pagamentos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pagamentos_Checkins_CheckinsId",
                        column: x => x.CheckinsId,
                        principalTable: "Checkins",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Pagamentos_Hospedagems_HospedagensId",
                        column: x => x.HospedagensId,
                        principalTable: "Hospedagems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Pagamentos_Hospedes_HospedesId",
                        column: x => x.HospedesId,
                        principalTable: "Hospedes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Pagamentos_Utilizador_UtilizadoresId",
                        column: x => x.UtilizadoresId,
                        principalTable: "Utilizador",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ItemPedidos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdProduto = table.Column<int>(type: "int", nullable: false),
                    Quantidade = table.Column<int>(type: "int", nullable: false),
                    Preco = table.Column<float>(type: "real", nullable: false),
                    PedidosId = table.Column<int>(type: "int", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IdTenant = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemPedidos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemPedidos_Pedidos_PedidosId",
                        column: x => x.PedidosId,
                        principalTable: "Pedidos",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "LancamentoCaixas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DataHoraLancamento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataHoraVencimento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Valor = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ValorPago = table.Column<float>(type: "real", nullable: false),
                    Troco = table.Column<float>(type: "real", nullable: false),
                    Observacao = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CaixasId = table.Column<int>(type: "int", nullable: false),
                    TipoPagamentosId = table.Column<int>(type: "int", nullable: false),
                    PagamentosId = table.Column<int>(type: "int", nullable: false),
                    UtilizadoresId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    PlanoDeContasId = table.Column<int>(type: "int", nullable: false),
                    ReferenciaId = table.Column<int>(type: "int", nullable: false),
                    TipoLancamento = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IdTenant = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LancamentoCaixas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LancamentoCaixas_Caixas_CaixasId",
                        column: x => x.CaixasId,
                        principalTable: "Caixas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LancamentoCaixas_Pagamentos_PagamentosId",
                        column: x => x.PagamentosId,
                        principalTable: "Pagamentos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LancamentoCaixas_PlanoDeContas_PlanoDeContasId",
                        column: x => x.PlanoDeContasId,
                        principalTable: "PlanoDeContas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LancamentoCaixas_TipoPagamentos_TipoPagamentosId",
                        column: x => x.TipoPagamentosId,
                        principalTable: "TipoPagamentos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LancamentoCaixas_Utilizador_UtilizadoresId",
                        column: x => x.UtilizadoresId,
                        principalTable: "Utilizador",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "AppConfig",
                columns: new[] { "Id", "Key", "Value" },
                values: new object[,]
                {
                    { 1, "smsServiceEnabled", "1" },
                    { 2, "emailServiceEnabled", "1" },
                    { 3, "emailSMTPServer", "smtp.gmail.com" },
                    { 4, "emailSMTPPort", "587" },
                    { 5, "emailUser", "ghotelweb@gmail.com" },
                    { 6, "emailPassword", "vztxbucwohdiolnq" },
                    { 7, "emailFrom", "ghotelweb@gmail.com" },
                    { 8, "emailBodyRegisteredBO", "<p>Olá,</p><br><p>encontra-se registado no backoffice do GHOTEL.</p><p>Por favor confirme o registo clicando no seguinte <a href={0}>confirmar</a></p><br><p>Atentamente</p><p>GHOTEL</p>" },
                    { 9, "emailSubjectRegisteredBO", "Bem-vindo ao GHOTEL" },
                    { 10, "emailConfirmUrl", "http://localhost:5055/api/usuario/confirm-email?email={0}&token={1}" },
                    { 11, "emailBodyBeforeChangeEmail", "<p>Olá,</p><br><p>foi efetuado um pedido de mudança do seu email na plataforma GHOTEL.</p><p>Por favor confirme a alteração para o novo email:{0} clicando no seguinte <a href={1}>confirmar</a></p><br><p>Atentamente</p><p>GHOTEL</p>" },
                    { 12, "emailSubjectBeforeChangeEmail", "GHOTEL - Pedido de alteração de email" },
                    { 13, "emailChangeUrl", "https://GHOTEL-dev.northeurope.cloudapp.azure.com:447/internal_api/Account/change-email?email={0}&token={1}" },
                    { 14, "emailBodyAfterChangeEmail", "<p>Olá,</p><br><p>o seu email foi alterado com sucesso na plataforma GHOTEL.</p><br><p>Atentamente</p><p>GHOTEL</p>" },
                    { 15, "emailSubjectAfterChangeEmail", "GHOTEL - email alterado" },
                    { 16, "smsBodyConfirmation", "GHOTEL - Código de confirmação: {0}" },
                    { 17, "emailBodyRegisteredFO", "<p>Olá,</p><br><p>encontra-se registado a plataforma do GHOTEL.</p><p>Por favor confirme o email clicando no seguinte <a href={0}>confirmar</a></p><br><p>Atentamente</p><p>GHOTEL</p>" },
                    { 18, "emailSubjectRegisteredFO", "Bem-vindo ao GHOTEL" },
                    { 19, "smsBodyAfterConfirmation", "Bem-vindo ao GHOTEL." },
                    { 20, "smsApiLogin", "/user/login" },
                    { 21, "smsApiTestToken", "/sender-id/list-one" },
                    { 22, "smsApiUsername", "929011521" },
                    { 23, "smsApiPassword", "maptss12345" },
                    { 24, "smsSenderName", "INEFOP" },
                    { 25, "smsUserToken", "1a95521ba4c88479b24f59cd5a2a7b83929011521" },
                    { 26, "useSsl", "true" },
                    { 27, "backOfficeUrl", "http://oinquilino.ao" },
                    { 28, "frontOfficeUrl", "http://oinquilino.ao" },
                    { 29, "smsApiUri", "http://52.30.114.86:8080/mimosms/v1" },
                    { 30, "smsApiSendSMS", "/message/send" },
                    { 31, "smsBodyUserNotification", "Motivo: {0} - Assunto : {1}. {2}" },
                    { 32, "emailChangePassswordUrl", "http://localhost:5055/api/usuario/change-password?token={0}&username={1}" },
                    { 33, "emailBodyBeforeChangePassword", "<p>Olá,</p><br><p>Foi efectuado um pedido de mudança de palavra-passe na plataforma do GHOTEL.</p><p>Por favor confirme a alteração clicando em <a href={0}>confirmar</a></p><br><p>Atentamente</p><p>Equipa do Ghotel</p>" },
                    { 34, "emailSubjectBeforeChangePassword", "GHOTEL - Pedido de alteração de palavra-passe" }
                });

            migrationBuilder.InsertData(
                table: "Contas",
                columns: new[] { "Id", "CreatedBy", "DateCreated", "Descricao", "IdTenant", "IsActive", "LastModifiedDate" },
                values: new object[,]
                {
                    { 1, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "ENTRADA/RECEITA", 0, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "SAIDA/DESPESA", 0, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "DocumentosVendas",
                columns: new[] { "Documento", "Descricao", "Diario", "Estado", "PagarReceber", "TipoConta" },
                values: new object[,]
                {
                    { "AA", "Alienação de activo", null, null, null, null },
                    { "AC", "Aviso de cobrança", "54", "PEN", "R", "CCT" },
                    { "AF", "Factura/recibo (autofacturação)", null, null, "R", null },
                    { "AR", "Aviso de cobrança/recibo", null, null, "R", null },
                    { "DA", "Devoluçao de activo", null, null, null, null },
                    { "FG", "Factura global", "52", "PEN", "R", "CCC" },
                    { "FR", "Factura/recibo", "51", "PEN", "P", "CCC" },
                    { "FS", "Factura genérica", "51", "PEN", "R", "CCC" },
                    { "FT", "Factura", "51", null, "P", null },
                    { "GR", "Guia remessa", null, null, "R", null },
                    { "NC", "Nota Crédito", "55", "PEN", "P", "CCC" },
                    { "ND", "Nota Débito", "55", "PEN", "R", "CCC" },
                    { "RE", "Recibo", null, null, "R", null },
                    { "TD", "Talão de devolução", null, null, null, null },
                    { "TS", "Talão de serviços prestados", "56", "PEN", "R", "CCC" },
                    { "TV", "Talão de venda", "1", null, "R", null },
                    { "VD", "Venda-a-Dinheiro", null, null, "R", null }
                });

            migrationBuilder.InsertData(
                table: "MotivoViagens",
                columns: new[] { "Id", "CreatedBy", "DateCreated", "Descricao", "IdTenant", "IsActive", "LastModifiedDate" },
                values: new object[,]
                {
                    { 1, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Negócios", 0, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Lazer", 0, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 3, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Família", 0, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 4, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Férias", 0, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 5, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Estudos", 0, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 6, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Outros", 0, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "Paises",
                columns: new[] { "Id", "CreatedBy", "DateCreated", "IdTenant", "IsActive", "LastModifiedDate", "Nacionalidade", "Nome" },
                values: new object[,]
                {
                    { 1, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Brasil" },
                    { 2, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Estados Unidos" },
                    { 3, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Canadá" },
                    { 4, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Argentina" },
                    { 5, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Chile" },
                    { 6, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Uruguai" },
                    { 7, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Paraguai" },
                    { 8, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Bolívia" },
                    { 9, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Peru" },
                    { 10, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Equador" },
                    { 11, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Colômbia" },
                    { 12, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Venezuela" },
                    { 13, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Guiana" },
                    { 14, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Suriname" },
                    { 15, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "França" },
                    { 16, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Espanha" },
                    { 17, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Itália" },
                    { 18, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Alemanha" },
                    { 19, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Reino Unido" },
                    { 20, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Portugal" },
                    { 21, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Suíça" },
                    { 22, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Austrália" },
                    { 23, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Nova Zelândia" },
                    { 24, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Japão" },
                    { 25, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "China" },
                    { 26, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Coreia do Sul" },
                    { 27, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Índia" },
                    { 28, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Paquistão" },
                    { 29, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Bangladesh" },
                    { 30, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Tailândia" },
                    { 31, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Filipinas" },
                    { 32, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Vietnã" },
                    { 33, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "México" },
                    { 34, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Angola" },
                    { 35, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Congo Democrático" },
                    { 36, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Congo" },
                    { 37, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Moçambique" },
                    { 38, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Ruanda" }
                });

            migrationBuilder.InsertData(
                table: "Perfis",
                columns: new[] { "Id", "CreatedBy", "DateCreated", "Descricao", "IdTenant", "IsActive", "LastModifiedDate" },
                values: new object[] { 1, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Administrador", 0, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.InsertData(
                table: "PonteDeVendas",
                columns: new[] { "Id", "CreatedBy", "IdTenant", "IsActive", "LastModifiedDate", "Nome" },
                values: new object[,]
                {
                    { 1, null, 0, true, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "HOTEL" },
                    { 2, null, 0, true, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "LAVANDARIA" },
                    { 3, null, 0, true, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "RESTAURANTE" },
                    { 4, null, 0, true, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "FRIGOBAR" },
                    { 5, null, 0, true, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "BAR" }
                });

            migrationBuilder.InsertData(
                table: "ProductTypes",
                columns: new[] { "ProductTypeCode", "ProductTypeDescription" },
                values: new object[,]
                {
                    { "E", "Imposto Especiais de Consumo - (ex.: IEC)" },
                    { "I", "Imposto, taxas e encargos parafiscais " },
                    { "O", "Outros (portes debitados, adiantamentos recebidos)" },
                    { "P", "Produtos" },
                    { "S", "Serviços" }
                });

            migrationBuilder.InsertData(
                table: "Role",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "1", null, "SuperAdmin", "SUPERADMIN" },
                    { "2", null, "Administrador", "ADMINISTRADOR" },
                    { "3", null, "Hotel", "HOTEL" },
                    { "4", null, "Finanças", "FINANÇAS" },
                    { "5", null, "Lavandaria", "LAVANDARIA" }
                });

            migrationBuilder.InsertData(
                table: "TipoApartamentos",
                columns: new[] { "Id", "CreatedBy", "DateCreated", "Descricao", "Domingo", "IdTenant", "IsActive", "LastModifiedDate", "MobiliaTipoApartamentoId", "Quarta", "Quinta", "Sabado", "Segunda", "Sexta", "Terca", "ValorDiariaDouble", "ValorDiariaQuadruple", "ValorDiariaSingle", "ValorDiariaTriple", "ValorDuasHora", "ValorNoite", "ValorQuatroHora", "ValorTresHora", "ValorUmaHora" },
                values: new object[,]
                {
                    { 1, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Casal Simples", 0f, 0, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 0f, 0f, 0f, 0f, 0f, 0f, 10000f, 10000f, 10000f, 10000f, 10000f, 1000f, 1000f, 10000f, 10000f },
                    { 2, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Casal Completo", 0f, 0, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 0f, 0f, 0f, 0f, 0f, 0f, 10000f, 10000f, 10000f, 10000f, 10000f, 1000f, 1000f, 10000f, 10000f },
                    { 3, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Duplo(BB)", 0f, 0, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 0f, 0f, 0f, 0f, 0f, 0f, 10000f, 10000f, 10000f, 10000f, 10000f, 1000f, 1000f, 10000f, 10000f },
                    { 4, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Quarto Médio", 0f, 0, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 0f, 0f, 0f, 0f, 0f, 0f, 10000f, 10000f, 10000f, 10000f, 10000f, 1000f, 1000f, 10000f, 10000f }
                });

            migrationBuilder.InsertData(
                table: "TipoGovernancas",
                columns: new[] { "Id", "CreatedBy", "DateCreated", "Descricao", "IdTenant", "IsActive", "LastModifiedDate" },
                values: new object[,]
                {
                    { 1, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Arrumação", 0, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Limpeza", 0, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 3, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Sujo", 0, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 4, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Manutenção", 0, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "TipoHospedagens",
                columns: new[] { "Id", "CreatedBy", "DateCreated", "Descricao", "IdTenant", "IsActive", "LastModifiedDate", "Valor" },
                values: new object[,]
                {
                    { 1, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "DIARIA", 0, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0m },
                    { 2, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "HORA", 0, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0m },
                    { 3, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "NOITE", 0, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0m },
                    { 4, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "ESPECIAL", 0, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0m },
                    { 5, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "DIARIA(PA)", 0, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 15000m },
                    { 6, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "MENSAL", 0, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0m },
                    { 7, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "CAMA EXTRA", 0, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0m },
                    { 8, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "DIARIA(SPA)", 0, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 10000m }
                });

            migrationBuilder.InsertData(
                table: "TipoPagamentos",
                columns: new[] { "Id", "CreatedBy", "DateCreated", "Descricao", "IdTenant", "IsActive", "LastModifiedDate" },
                values: new object[,]
                {
                    { 1, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "MultiCaixa", 0, true, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Cheque bancário", 0, true, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 3, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Compensação de saldos em conta corrente", 0, true, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 4, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Referência de pagamento para Multicaixa", 0, true, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 5, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Numerário", 0, true, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 6, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Transferência bancária", 0, true, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "TipoRecibos",
                columns: new[] { "Id", "CreatedBy", "DateCreated", "Descricao", "IdTenant", "IsActive", "LastModifiedDate", "RPT" },
                values: new object[,]
                {
                    { 1, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Recibo Grande", 0, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "CrReciboNovo.rpt" },
                    { 2, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Recibo Pequeno", 0, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "CrReciboP.rpt" },
                    { 3, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Ticket", 0, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "CrReciboP2.rpt" }
                });

            migrationBuilder.InsertData(
                table: "Apartamentos",
                columns: new[] { "Id", "CafeDaManha", "CheckinsId", "Codigo", "CodigoRamal", "CreatedBy", "DateCreated", "Frigobar", "IdTenant", "IsActive", "LastModifiedDate", "NaoPertube", "Observacao", "TipoApartamentosId", "TipoGovernancasId" },
                values: new object[,]
                {
                    { 1, 0, null, "Quarto-001", 0, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "N", 0, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "", 1, 1 },
                    { 2, 0, null, "Quarto-002", 0, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "N", 0, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "", 1, 1 },
                    { 3, 0, null, "Quarto-003", 0, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "N", 0, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "", 1, 1 },
                    { 4, 0, null, "Quarto-004", 0, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "N", 0, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "", 1, 1 },
                    { 5, 0, null, "Quarto-005", 0, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "N", 0, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "", 1, 1 },
                    { 6, 0, null, "Quarto-006", 0, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "N", 0, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "", 1, 1 },
                    { 7, 0, null, "Quarto-007", 0, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "N", 0, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "", 1, 1 },
                    { 8, 0, null, "Quarto-008", 0, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "N", 0, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "", 1, 1 },
                    { 9, 0, null, "Quarto-009", 0, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "N", 0, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "", 1, 1 },
                    { 10, 0, null, "Quarto-010", 0, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "N", 0, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "", 1, 1 },
                    { 11, 0, null, "Quarto-011", 0, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "N", 0, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "", 1, 1 },
                    { 12, 0, null, "Quarto-012", 0, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "N", 0, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "", 1, 1 },
                    { 13, 0, null, "Quarto-013", 0, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "N", 0, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "", 1, 1 },
                    { 14, 0, null, "Quarto-014", 0, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "N", 0, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "", 1, 1 },
                    { 15, 0, null, "Quarto-015", 0, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "N", 0, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "", 1, 1 },
                    { 16, 0, null, "Quarto-016", 0, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "N", 0, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "", 1, 1 }
                });

            migrationBuilder.InsertData(
                table: "PlanoDeContas",
                columns: new[] { "Id", "ContaId", "ContasId", "CreatedBy", "DateCreated", "Descricao", "IdTenant", "IsActive", "LastModifiedDate" },
                values: new object[,]
                {
                    { 1, null, 1, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Recebimento Diversos", 0, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, null, 1, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Venda", 0, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 3, null, 1, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Abertura de Caixa", 0, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 4, null, 1, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Restaurante", 0, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 5, null, 1, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Diarias", 0, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 6, null, 1, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Lavandaria", 0, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 7, null, 1, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Bar", 0, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 8, null, 1, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Telefone", 0, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 9, null, 1, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Transporte", 0, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 10, null, 1, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Extra Alojamento", 0, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 11, null, 1, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Pagamento de Prestação de Serviço", 0, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 12, null, 1, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Material de Escritório", 0, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 13, null, 1, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Compra de Mercadoria", 0, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 14, null, 1, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Pagamentos Diversos", 0, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Acessos_PerfilsId",
                table: "Acessos",
                column: "PerfilsId");

            migrationBuilder.CreateIndex(
                name: "IX_Apartamentos_CheckinsId",
                table: "Apartamentos",
                column: "CheckinsId");

            migrationBuilder.CreateIndex(
                name: "IX_Apartamentos_TipoApartamentosId",
                table: "Apartamentos",
                column: "TipoApartamentosId");

            migrationBuilder.CreateIndex(
                name: "IX_Apartamentos_TipoGovernancasId",
                table: "Apartamentos",
                column: "TipoGovernancasId");

            migrationBuilder.CreateIndex(
                name: "IX_ApartamentosReservados_ApartamentosId",
                table: "ApartamentosReservados",
                column: "ApartamentosId");

            migrationBuilder.CreateIndex(
                name: "IX_ApartamentosReservados_ClientesId",
                table: "ApartamentosReservados",
                column: "ClientesId");

            migrationBuilder.CreateIndex(
                name: "IX_ApartamentosReservados_ReservasId",
                table: "ApartamentosReservados",
                column: "ReservasId");

            migrationBuilder.CreateIndex(
                name: "IX_ApartamentosReservados_TipoHospedagensId",
                table: "ApartamentosReservados",
                column: "TipoHospedagensId");

            migrationBuilder.CreateIndex(
                name: "IX_ApartamentosReservados_UtilizadoresId",
                table: "ApartamentosReservados",
                column: "UtilizadoresId");

            migrationBuilder.CreateIndex(
                name: "IX_Caixas_UtilizadoresId",
                table: "Caixas",
                column: "UtilizadoresId");

            migrationBuilder.CreateIndex(
                name: "IX_Checkins_CaixaId",
                table: "Checkins",
                column: "CaixaId");

            migrationBuilder.CreateIndex(
                name: "IX_Checkins_IdCaixaCheckin",
                table: "Checkins",
                column: "IdCaixaCheckin");

            migrationBuilder.CreateIndex(
                name: "IX_Checkins_IdCaixaCheckOut",
                table: "Checkins",
                column: "IdCaixaCheckOut");

            migrationBuilder.CreateIndex(
                name: "IX_Checkins_IdUtilizadorCheckin",
                table: "Checkins",
                column: "IdUtilizadorCheckin");

            migrationBuilder.CreateIndex(
                name: "IX_Checkins_IdUtilizadorCheckOut",
                table: "Checkins",
                column: "IdUtilizadorCheckOut");

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_EmpresasId",
                table: "Clientes",
                column: "EmpresasId");

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_PaisId",
                table: "Clientes",
                column: "PaisId");

            migrationBuilder.CreateIndex(
                name: "IX_Despertadores_CheckinsId",
                table: "Despertadores",
                column: "CheckinsId");

            migrationBuilder.CreateIndex(
                name: "IX_FacturaDivididas_CheckinsId",
                table: "FacturaDivididas",
                column: "CheckinsId");

            migrationBuilder.CreateIndex(
                name: "IX_FacturaDivididas_HospedagensId",
                table: "FacturaDivididas",
                column: "HospedagensId");

            migrationBuilder.CreateIndex(
                name: "IX_FacturaEmpresas_CheckinsId",
                table: "FacturaEmpresas",
                column: "CheckinsId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FacturaEmpresas_EmpresasId",
                table: "FacturaEmpresas",
                column: "EmpresasId");

            migrationBuilder.CreateIndex(
                name: "IX_Governancas_TipoGovernancasId",
                table: "Governancas",
                column: "TipoGovernancasId");

            migrationBuilder.CreateIndex(
                name: "IX_Historicos_CheckinsId",
                table: "Historicos",
                column: "CheckinsId");

            migrationBuilder.CreateIndex(
                name: "IX_Historicos_HospedagemId",
                table: "Historicos",
                column: "HospedagemId");

            migrationBuilder.CreateIndex(
                name: "IX_Historicos_UtilizadoresId",
                table: "Historicos",
                column: "UtilizadoresId");

            migrationBuilder.CreateIndex(
                name: "IX_Hospedagems_ApartamentosId",
                table: "Hospedagems",
                column: "ApartamentosId");

            migrationBuilder.CreateIndex(
                name: "IX_Hospedagems_CheckinsId",
                table: "Hospedagems",
                column: "CheckinsId");

            migrationBuilder.CreateIndex(
                name: "IX_Hospedagems_EmpresasId",
                table: "Hospedagems",
                column: "EmpresasId");

            migrationBuilder.CreateIndex(
                name: "IX_Hospedagems_MotivoViagensId",
                table: "Hospedagems",
                column: "MotivoViagensId");

            migrationBuilder.CreateIndex(
                name: "IX_Hospedagems_TipoHospedagensId",
                table: "Hospedagems",
                column: "TipoHospedagensId");

            migrationBuilder.CreateIndex(
                name: "IX_Hospedes_CheckinsId",
                table: "Hospedes",
                column: "CheckinsId");

            migrationBuilder.CreateIndex(
                name: "IX_Hospedes_ClientesId",
                table: "Hospedes",
                column: "ClientesId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemPedidos_PedidosId",
                table: "ItemPedidos",
                column: "PedidosId");

            migrationBuilder.CreateIndex(
                name: "IX_LancamentoCaixas_CaixasId",
                table: "LancamentoCaixas",
                column: "CaixasId");

            migrationBuilder.CreateIndex(
                name: "IX_LancamentoCaixas_PagamentosId",
                table: "LancamentoCaixas",
                column: "PagamentosId");

            migrationBuilder.CreateIndex(
                name: "IX_LancamentoCaixas_PlanoDeContasId",
                table: "LancamentoCaixas",
                column: "PlanoDeContasId");

            migrationBuilder.CreateIndex(
                name: "IX_LancamentoCaixas_TipoPagamentosId",
                table: "LancamentoCaixas",
                column: "TipoPagamentosId");

            migrationBuilder.CreateIndex(
                name: "IX_LancamentoCaixas_UtilizadoresId",
                table: "LancamentoCaixas",
                column: "UtilizadoresId");

            migrationBuilder.CreateIndex(
                name: "IX_lavandariaItens_LavandariasId",
                table: "lavandariaItens",
                column: "LavandariasId");

            migrationBuilder.CreateIndex(
                name: "IX_lavandariaItens_UtilizadoresId",
                table: "lavandariaItens",
                column: "UtilizadoresId");

            migrationBuilder.CreateIndex(
                name: "IX_lavandarias_ClientesId",
                table: "lavandarias",
                column: "ClientesId");

            migrationBuilder.CreateIndex(
                name: "IX_lavandarias_UtilizadoresId",
                table: "lavandarias",
                column: "UtilizadoresId");

            migrationBuilder.CreateIndex(
                name: "IX_MenuItem_ParentId",
                table: "MenuItem",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_MenuRole_RoleId",
                table: "MenuRole",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_MobiliaApartamentos_ApartamentosId",
                table: "MobiliaApartamentos",
                column: "ApartamentosId");

            migrationBuilder.CreateIndex(
                name: "IX_MobiliaApartamentos_PatrimoniosId",
                table: "MobiliaApartamentos",
                column: "PatrimoniosId");

            migrationBuilder.CreateIndex(
                name: "IX_MobiliaApartamentos_TipoApartamentosId",
                table: "MobiliaApartamentos",
                column: "TipoApartamentosId");

            migrationBuilder.CreateIndex(
                name: "IX_MobiliaTipoApartamentoPatrimonio_patrimoniosId",
                table: "MobiliaTipoApartamentoPatrimonio",
                column: "patrimoniosId");

            migrationBuilder.CreateIndex(
                name: "IX_Pagamentos_CheckinsId",
                table: "Pagamentos",
                column: "CheckinsId");

            migrationBuilder.CreateIndex(
                name: "IX_Pagamentos_HospedagensId",
                table: "Pagamentos",
                column: "HospedagensId");

            migrationBuilder.CreateIndex(
                name: "IX_Pagamentos_HospedesId",
                table: "Pagamentos",
                column: "HospedesId");

            migrationBuilder.CreateIndex(
                name: "IX_Pagamentos_UtilizadoresId",
                table: "Pagamentos",
                column: "UtilizadoresId");

            migrationBuilder.CreateIndex(
                name: "IX_Pedidos_HospedesId",
                table: "Pedidos",
                column: "HospedesId");

            migrationBuilder.CreateIndex(
                name: "IX_Pedidos_PonteDeVendasId",
                table: "Pedidos",
                column: "PonteDeVendasId");

            migrationBuilder.CreateIndex(
                name: "IX_PlanoDeContas_ContaId",
                table: "PlanoDeContas",
                column: "ContaId");

            migrationBuilder.CreateIndex(
                name: "IX_PlanoDeContas_ContasId",
                table: "PlanoDeContas",
                column: "ContasId");

            migrationBuilder.CreateIndex(
                name: "IX_Produtos_PlanoDeContasId",
                table: "Produtos",
                column: "PlanoDeContasId");

            migrationBuilder.CreateIndex(
                name: "IX_Produtos_ProductTypesProductTypeCode",
                table: "Produtos",
                column: "ProductTypesProductTypeCode");

            migrationBuilder.CreateIndex(
                name: "IX_Produtos_TaxExemptionReasonTaxExemptionCode",
                table: "Produtos",
                column: "TaxExemptionReasonTaxExemptionCode");

            migrationBuilder.CreateIndex(
                name: "IX_Produtos_TaxTableEntryId",
                table: "Produtos",
                column: "TaxTableEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_Produtos_TipoProdutosId",
                table: "Produtos",
                column: "TipoProdutosId");

            migrationBuilder.CreateIndex(
                name: "IX_ProdutoStocks_ProdutoId",
                table: "ProdutoStocks",
                column: "ProdutoId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservas_EmpresaId",
                table: "Reservas",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservas_UtilizadoresId",
                table: "Reservas",
                column: "UtilizadoresId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "Role",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_RoleClaims_RoleId",
                table: "RoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Tax_TaxType",
                table: "Tax",
                column: "TaxType");

            migrationBuilder.CreateIndex(
                name: "IX_TaxExemptionReasons_TaxCode",
                table: "TaxExemptionReasons",
                column: "TaxCode");

            migrationBuilder.CreateIndex(
                name: "IX_TaxTableEntry_TaxType",
                table: "TaxTableEntry",
                column: "TaxType");

            migrationBuilder.CreateIndex(
                name: "IX_TipoApartamentos_MobiliaTipoApartamentoId",
                table: "TipoApartamentos",
                column: "MobiliaTipoApartamentoId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "Utilizador",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_Utilizador_RefreshTokenId",
                table: "Utilizador",
                column: "RefreshTokenId");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "Utilizador",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_UtilizadorClaims_UserId",
                table: "UtilizadorClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UtilizadorLogins_UserId",
                table: "UtilizadorLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UtilizadorRoles_RoleId",
                table: "UtilizadorRoles",
                column: "RoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Acessos");

            migrationBuilder.DropTable(
                name: "ApartamentosReservados");

            migrationBuilder.DropTable(
                name: "AppConfig");

            migrationBuilder.DropTable(
                name: "Despertadores");

            migrationBuilder.DropTable(
                name: "DocumentosVendas");

            migrationBuilder.DropTable(
                name: "FacturaDivididas");

            migrationBuilder.DropTable(
                name: "FacturaEmpresas");

            migrationBuilder.DropTable(
                name: "Governancas");

            migrationBuilder.DropTable(
                name: "Historicos");

            migrationBuilder.DropTable(
                name: "ItemPedidos");

            migrationBuilder.DropTable(
                name: "LancamentoCaixas");

            migrationBuilder.DropTable(
                name: "lavandariaItens");

            migrationBuilder.DropTable(
                name: "MenuItem");

            migrationBuilder.DropTable(
                name: "MenuRole");

            migrationBuilder.DropTable(
                name: "MobiliaApartamentos");

            migrationBuilder.DropTable(
                name: "MobiliaTipoApartamentoPatrimonio");

            migrationBuilder.DropTable(
                name: "Params");

            migrationBuilder.DropTable(
                name: "ProdutoStocks");

            migrationBuilder.DropTable(
                name: "RoleClaims");

            migrationBuilder.DropTable(
                name: "Serial");

            migrationBuilder.DropTable(
                name: "Series");

            migrationBuilder.DropTable(
                name: "Tax");

            migrationBuilder.DropTable(
                name: "TaxAccountingBasis");

            migrationBuilder.DropTable(
                name: "TipoRecibos");

            migrationBuilder.DropTable(
                name: "UtilizadorClaims");

            migrationBuilder.DropTable(
                name: "UtilizadorLogins");

            migrationBuilder.DropTable(
                name: "UtilizadorRoles");

            migrationBuilder.DropTable(
                name: "UtilizadorTokens");

            migrationBuilder.DropTable(
                name: "Perfis");

            migrationBuilder.DropTable(
                name: "Reservas");

            migrationBuilder.DropTable(
                name: "Pedidos");

            migrationBuilder.DropTable(
                name: "Pagamentos");

            migrationBuilder.DropTable(
                name: "TipoPagamentos");

            migrationBuilder.DropTable(
                name: "lavandarias");

            migrationBuilder.DropTable(
                name: "AppMenu");

            migrationBuilder.DropTable(
                name: "Patrimonios");

            migrationBuilder.DropTable(
                name: "Produtos");

            migrationBuilder.DropTable(
                name: "Role");

            migrationBuilder.DropTable(
                name: "PonteDeVendas");

            migrationBuilder.DropTable(
                name: "Hospedagems");

            migrationBuilder.DropTable(
                name: "Hospedes");

            migrationBuilder.DropTable(
                name: "PlanoDeContas");

            migrationBuilder.DropTable(
                name: "ProductTypes");

            migrationBuilder.DropTable(
                name: "TaxExemptionReasons");

            migrationBuilder.DropTable(
                name: "TipoProdutos");

            migrationBuilder.DropTable(
                name: "Apartamentos");

            migrationBuilder.DropTable(
                name: "MotivoViagens");

            migrationBuilder.DropTable(
                name: "TipoHospedagens");

            migrationBuilder.DropTable(
                name: "Clientes");

            migrationBuilder.DropTable(
                name: "Contas");

            migrationBuilder.DropTable(
                name: "TaxTableEntry");

            migrationBuilder.DropTable(
                name: "Checkins");

            migrationBuilder.DropTable(
                name: "TipoApartamentos");

            migrationBuilder.DropTable(
                name: "TipoGovernancas");

            migrationBuilder.DropTable(
                name: "Empresas");

            migrationBuilder.DropTable(
                name: "Paises");

            migrationBuilder.DropTable(
                name: "TaxTypes");

            migrationBuilder.DropTable(
                name: "Caixas");

            migrationBuilder.DropTable(
                name: "MobiliaTipoApartamentos");

            migrationBuilder.DropTable(
                name: "Utilizador");

            migrationBuilder.DropTable(
                name: "RefreshToken");
        }
    }
}
