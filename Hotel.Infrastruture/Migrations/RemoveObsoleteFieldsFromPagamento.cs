using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Hotel.Infrastruture.Migrations
{
    public partial class RemoveObsoleteFieldsFromPagamento : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Remove foreign key constraint se existir
            migrationBuilder.DropForeignKey(
                name: "FK_Pagamentos_Checkins_CheckinsId",
                table: "Pagamentos");

            migrationBuilder.DropIndex(
                name: "IX_Pagamentos_CheckinsId",
                table: "Pagamentos");

            // Remove colunas obsoletas
            migrationBuilder.DropColumn(
                name: "CheckinsId",
                table: "Pagamentos");

            migrationBuilder.DropColumn(
                name: "IdVenda",
                table: "Pagamentos");

            migrationBuilder.DropColumn(
                name: "HospedagensId",
                table: "Pagamentos");

            // Adiciona novos campos se ainda não existirem
            migrationBuilder.AddColumn<string>(
                name: "Origem",
                table: "Pagamentos",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "OrigemId",
                table: "Pagamentos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Observacao",
                table: "Pagamentos",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            // Cria índices
            migrationBuilder.CreateIndex(
                name: "IX_Pagamento_Origem_OrigemId",
                table: "Pagamentos",
                columns: new[] { "Origem", "OrigemId" });

            migrationBuilder.CreateIndex(
                name: "IX_Pagamento_HospedesId",
                table: "Pagamentos",
                column: "HospedesId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Pagamento_Origem_OrigemId",
                table: "Pagamentos");

            migrationBuilder.DropIndex(
                name: "IX_Pagamento_HospedesId",
                table: "Pagamentos");

            migrationBuilder.DropColumn(
                name: "Origem",
                table: "Pagamentos");

            migrationBuilder.DropColumn(
                name: "OrigemId",
                table: "Pagamentos");

            migrationBuilder.DropColumn(
                name: "Observacao",
                table: "Pagamentos");

            migrationBuilder.AddColumn<int>(
                name: "CheckinsId",
                table: "Pagamentos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "IdVenda",
                table: "Pagamentos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "HospedagensId",
                table: "Pagamentos",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pagamentos_CheckinsId",
                table: "Pagamentos",
                column: "CheckinsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Pagamentos_Checkins_CheckinsId",
                table: "Pagamentos",
                column: "CheckinsId",
                principalTable: "Checkins",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}