using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HelpDesk.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRolePermission : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
    }
}
