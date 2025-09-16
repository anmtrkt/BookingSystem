using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class newUserInfos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Something",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<Guid>(
                name: "MeetingId",
                table: "ScheduleTimeRanges",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "AspNetUsers",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(128)",
                oldMaxLength: 128);

            migrationBuilder.AddColumn<byte>(
                name: "PriorityLevel",
                table: "AspNetUsers",
                type: "smallint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleTimeRanges_MeetingId",
                table: "ScheduleTimeRanges",
                column: "MeetingId");

            migrationBuilder.AddForeignKey(
                name: "FK_ScheduleTimeRanges_Meetings_MeetingId",
                table: "ScheduleTimeRanges",
                column: "MeetingId",
                principalTable: "Meetings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ScheduleTimeRanges_Meetings_MeetingId",
                table: "ScheduleTimeRanges");

            migrationBuilder.DropIndex(
                name: "IX_ScheduleTimeRanges_MeetingId",
                table: "ScheduleTimeRanges");

            migrationBuilder.DropColumn(
                name: "MeetingId",
                table: "ScheduleTimeRanges");

            migrationBuilder.DropColumn(
                name: "PriorityLevel",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "AspNetUsers",
                type: "character varying(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "Something",
                table: "AspNetUsers",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");
        }
    }
}
