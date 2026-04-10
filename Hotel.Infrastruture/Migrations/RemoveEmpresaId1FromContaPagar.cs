using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Hotel.Infrastruture.Migrations
{
    public partial class RemoveEmpresaId1FromContaPagar : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmpresaId1",
                table: "ContasPagar");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EmpresaId1",
                table: "ContasPagar",
                type: "int",
                nullable: true);
        }
    }
}