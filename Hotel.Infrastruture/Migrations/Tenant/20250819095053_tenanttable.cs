using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hotel.Infrastruture.Migrations.Tenant
{
    /// <inheritdoc />
    public partial class tenanttable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Tenant",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DatabaseServerName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DatabaseName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConnectionString = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Metadata_Region = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Metadata_MaxUsers = table.Column<int>(type: "int", nullable: true),
                    Metadata_IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    Metadata_Prazo = table.Column<int>(type: "int", nullable: true),
                    Metadata_KeySerial = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Metadata_CustomSettings = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tenant", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tenant");
        }
    }
}
