using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class thirdMig_10 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Meetings_Room_RoomId",
                table: "Meetings");

            migrationBuilder.DropForeignKey(
                name: "FK_Office_Organizations_OrganizationId",
                table: "Office");

            migrationBuilder.DropForeignKey(
                name: "FK_Room_Office_OfficeId",
                table: "Room");

            migrationBuilder.DropForeignKey(
                name: "FK_Room_Schedule_ScheduleId",
                table: "Room");

            migrationBuilder.DropForeignKey(
                name: "FK_ScheduleTimeRanges_Schedule_ScheduleId",
                table: "ScheduleTimeRanges");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Schedule",
                table: "Schedule");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Room",
                table: "Room");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Office",
                table: "Office");

            migrationBuilder.DropColumn(
                name: "NormalizedName",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "CreatorName",
                table: "Meetings");

            migrationBuilder.RenameTable(
                name: "Schedule",
                newName: "Schedules");

            migrationBuilder.RenameTable(
                name: "Room",
                newName: "Rooms");

            migrationBuilder.RenameTable(
                name: "Office",
                newName: "Offices");

            migrationBuilder.RenameIndex(
                name: "IX_Room_ScheduleId",
                table: "Rooms",
                newName: "IX_Rooms_ScheduleId");

            migrationBuilder.RenameIndex(
                name: "IX_Room_OfficeId",
                table: "Rooms",
                newName: "IX_Rooms_OfficeId");

            migrationBuilder.RenameIndex(
                name: "IX_Office_OrganizationId",
                table: "Offices",
                newName: "IX_Offices_OrganizationId");

            migrationBuilder.AlterColumn<Guid>(
                name: "OrganizationId",
                table: "Offices",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Schedules",
                table: "Schedules",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Rooms",
                table: "Rooms",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Offices",
                table: "Offices",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Meetings_Rooms_RoomId",
                table: "Meetings",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Offices_Organizations_OrganizationId",
                table: "Offices",
                column: "OrganizationId",
                principalTable: "Organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Rooms_Offices_OfficeId",
                table: "Rooms",
                column: "OfficeId",
                principalTable: "Offices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Rooms_Schedules_ScheduleId",
                table: "Rooms",
                column: "ScheduleId",
                principalTable: "Schedules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ScheduleTimeRanges_Schedules_ScheduleId",
                table: "ScheduleTimeRanges",
                column: "ScheduleId",
                principalTable: "Schedules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Meetings_Rooms_RoomId",
                table: "Meetings");

            migrationBuilder.DropForeignKey(
                name: "FK_Offices_Organizations_OrganizationId",
                table: "Offices");

            migrationBuilder.DropForeignKey(
                name: "FK_Rooms_Offices_OfficeId",
                table: "Rooms");

            migrationBuilder.DropForeignKey(
                name: "FK_Rooms_Schedules_ScheduleId",
                table: "Rooms");

            migrationBuilder.DropForeignKey(
                name: "FK_ScheduleTimeRanges_Schedules_ScheduleId",
                table: "ScheduleTimeRanges");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Schedules",
                table: "Schedules");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Rooms",
                table: "Rooms");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Offices",
                table: "Offices");

            migrationBuilder.RenameTable(
                name: "Schedules",
                newName: "Schedule");

            migrationBuilder.RenameTable(
                name: "Rooms",
                newName: "Room");

            migrationBuilder.RenameTable(
                name: "Offices",
                newName: "Office");

            migrationBuilder.RenameIndex(
                name: "IX_Rooms_ScheduleId",
                table: "Room",
                newName: "IX_Room_ScheduleId");

            migrationBuilder.RenameIndex(
                name: "IX_Rooms_OfficeId",
                table: "Room",
                newName: "IX_Room_OfficeId");

            migrationBuilder.RenameIndex(
                name: "IX_Offices_OrganizationId",
                table: "Office",
                newName: "IX_Office_OrganizationId");

            migrationBuilder.AddColumn<string>(
                name: "NormalizedName",
                table: "Organizations",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CreatorName",
                table: "Meetings",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<Guid>(
                name: "OrganizationId",
                table: "Office",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Schedule",
                table: "Schedule",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Room",
                table: "Room",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Office",
                table: "Office",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Meetings_Room_RoomId",
                table: "Meetings",
                column: "RoomId",
                principalTable: "Room",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Office_Organizations_OrganizationId",
                table: "Office",
                column: "OrganizationId",
                principalTable: "Organizations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Room_Office_OfficeId",
                table: "Room",
                column: "OfficeId",
                principalTable: "Office",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Room_Schedule_ScheduleId",
                table: "Room",
                column: "ScheduleId",
                principalTable: "Schedule",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ScheduleTimeRanges_Schedule_ScheduleId",
                table: "ScheduleTimeRanges",
                column: "ScheduleId",
                principalTable: "Schedule",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
