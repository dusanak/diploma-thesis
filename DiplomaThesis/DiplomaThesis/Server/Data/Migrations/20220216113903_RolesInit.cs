using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiplomaThesis.Server.Data.Migrations
{
    public partial class RolesInit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "38b67b40-209d-4243-970a-32de97142cf5", "1bac38ef-b140-41c7-9a37-361d61bad862", "Process Architect", "PROCESS_ARCHITECT" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "9e9c6ff9-cc34-4cd8-b36c-c34d4d0b691f", "1db60e06-c4bd-42f4-b6c0-3a7c763f4d0e", "Admin", "ADMIN" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "38b67b40-209d-4243-970a-32de97142cf5");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "9e9c6ff9-cc34-4cd8-b36c-c34d4d0b691f");
        }
    }
}
