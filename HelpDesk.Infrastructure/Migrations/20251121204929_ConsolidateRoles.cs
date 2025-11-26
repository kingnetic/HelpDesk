using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HelpDesk.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ConsolidateRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RolePermissions_Role_RoleId",
                schema: "security",
                table: "RolePermissions");

            migrationBuilder.DropTable(
                name: "Role",
                schema: "security");

            migrationBuilder.DropIndex(
                name: "IX_RolePermissions_RoleId",
                schema: "security",
                table: "RolePermissions");

            migrationBuilder.DropColumn(
                name: "RoleId",
                schema: "security",
                table: "RolePermissions");

            migrationBuilder.AddColumn<string>(
                name: "RoleName",
                schema: "security",
                table: "RolePermissions",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RoleName",
                schema: "security",
                table: "RolePermissions");

            migrationBuilder.AddColumn<int>(
                name: "RoleId",
                schema: "security",
                table: "RolePermissions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Role",
                schema: "security",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_RoleId",
                schema: "security",
                table: "RolePermissions",
                column: "RoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_RolePermissions_Role_RoleId",
                schema: "security",
                table: "RolePermissions",
                column: "RoleId",
                principalSchema: "security",
                principalTable: "Role",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
