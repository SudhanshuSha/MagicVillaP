using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MagicVilla_WebAPI.Migrations
{
    public partial class UpdatedNameLocalUsers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_localUsersx",
                table: "localUsersx");

            migrationBuilder.RenameTable(
                name: "localUsersx",
                newName: "LocalUsers");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LocalUsers",
                table: "LocalUsers",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_LocalUsers",
                table: "LocalUsers");

            migrationBuilder.RenameTable(
                name: "LocalUsers",
                newName: "localUsersx");

            migrationBuilder.AddPrimaryKey(
                name: "PK_localUsersx",
                table: "localUsersx",
                column: "Id");
        }
    }
}
