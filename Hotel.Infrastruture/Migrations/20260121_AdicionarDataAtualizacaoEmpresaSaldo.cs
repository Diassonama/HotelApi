using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hotel.Infrastruture.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarDataAtualizacaoEmpresaSaldo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Adicionar coluna DataAtualizacao em EmpresaSaldos
            migrationBuilder.AddColumn<DateTime>(
                name: "DataAtualizacao",
                table: "EmpresaSaldos",
                type: "datetime2",
                nullable: false,
                defaultValue: DateTime.Now);

            // Adicionar coluna Data em EmpresaSaldoMovimentos se não existir
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
                               WHERE TABLE_NAME = 'EmpresaSaldoMovimentos' AND COLUMN_NAME = 'Data')
                BEGIN
                    ALTER TABLE EmpresaSaldoMovimentos 
                    ADD Data datetime2 NOT NULL DEFAULT GETDATE()
                END
            ");

            // Remover coluna redundante TipoLancamentoId se existir
            migrationBuilder.DropColumn(
                name: "TipoLancamentoId",
                table: "EmpresaSaldoMovimentos");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataAtualizacao",
                table: "EmpresaSaldos");

            migrationBuilder.DropColumn(
                name: "Data",
                table: "EmpresaSaldoMovimentos");

            migrationBuilder.AddColumn<int>(
                name: "TipoLancamentoId",
                table: "EmpresaSaldoMovimentos",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}