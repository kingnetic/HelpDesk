using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HelpDesk.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSoftDeleteToBaseEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                schema: "security",
                table: "UserSessions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "security",
                table: "UserSessions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                schema: "helpdesk",
                table: "TicketStatusHistory",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "helpdesk",
                table: "TicketStatusHistory",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                schema: "helpdesk",
                table: "Tickets",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "helpdesk",
                table: "Tickets",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                schema: "helpdesk",
                table: "TicketComments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "helpdesk",
                table: "TicketComments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                schema: "helpdesk",
                table: "TicketAuditLogs",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "helpdesk",
                table: "TicketAuditLogs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                schema: "catalog",
                table: "Catalogs",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "catalog",
                table: "Catalogs",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "security",
                table: "UserSessions");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "security",
                table: "UserSessions");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "helpdesk",
                table: "TicketStatusHistory");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "helpdesk",
                table: "TicketStatusHistory");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "helpdesk",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "helpdesk",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "helpdesk",
                table: "TicketComments");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "helpdesk",
                table: "TicketComments");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "helpdesk",
                table: "TicketAuditLogs");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "helpdesk",
                table: "TicketAuditLogs");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "catalog",
                table: "Catalogs");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "catalog",
                table: "Catalogs");
        }
    }
}
