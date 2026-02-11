using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class secondMig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Meeting_Room_RoomId",
                table: "Meeting");

            migrationBuilder.DropForeignKey(
                name: "FK_Meeting_User_CreatorId",
                table: "Meeting");

            migrationBuilder.DropForeignKey(
                name: "FK_User_Meeting_MeetingId",
                table: "User");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Meeting",
                table: "Meeting");

            migrationBuilder.RenameTable(
                name: "Meeting",
                newName: "Meetings");

            migrationBuilder.RenameIndex(
                name: "IX_Meeting_RoomId",
                table: "Meetings",
                newName: "IX_Meetings_RoomId");

            migrationBuilder.RenameIndex(
                name: "IX_Meeting_CreatorId",
                table: "Meetings",
                newName: "IX_Meetings_CreatorId");

            migrationBuilder.AddColumn<Guid>(
                name: "OrganizationId",
                table: "Office",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Meetings",
                table: "Meetings",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Organizations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    NormalizedName = table.Column<string>(type: "text", nullable: false),
                    IsArchive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organizations", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Office_OrganizationId",
                table: "Office",
                column: "OrganizationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Meetings_Room_RoomId",
                table: "Meetings",
                column: "RoomId",
                principalTable: "Room",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Meetings_User_CreatorId",
                table: "Meetings",
                column: "CreatorId",
                principalTable: "User",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Office_Organizations_OrganizationId",
                table: "Office",
                column: "OrganizationId",
                principalTable: "Organizations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_User_Meetings_MeetingId",
                table: "User",
                column: "MeetingId",
                principalTable: "Meetings",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Meetings_Room_RoomId",
                table: "Meetings");

            migrationBuilder.DropForeignKey(
                name: "FK_Meetings_User_CreatorId",
                table: "Meetings");

            migrationBuilder.DropForeignKey(
                name: "FK_Office_Organizations_OrganizationId",
                table: "Office");

            migrationBuilder.DropForeignKey(
                name: "FK_User_Meetings_MeetingId",
                table: "User");

            migrationBuilder.DropTable(
                name: "Organizations");

            migrationBuilder.DropIndex(
                name: "IX_Office_OrganizationId",
                table: "Office");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Meetings",
                table: "Meetings");

            migrationBuilder.DropColumn(
                name: "OrganizationId",
                table: "Office");

            migrationBuilder.RenameTable(
                name: "Meetings",
                newName: "Meeting");

            migrationBuilder.RenameIndex(
                name: "IX_Meetings_RoomId",
                table: "Meeting",
                newName: "IX_Meeting_RoomId");

            migrationBuilder.RenameIndex(
                name: "IX_Meetings_CreatorId",
                table: "Meeting",
                newName: "IX_Meeting_CreatorId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Meeting",
                table: "Meeting",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Meeting_Room_RoomId",
                table: "Meeting",
                column: "RoomId",
                principalTable: "Room",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Meeting_User_CreatorId",
                table: "Meeting",
                column: "CreatorId",
                principalTable: "User",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_User_Meeting_MeetingId",
                table: "User",
                column: "MeetingId",
                principalTable: "Meeting",
                principalColumn: "Id");
        }
    }
}
