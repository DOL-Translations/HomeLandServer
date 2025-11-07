using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fragment.NetSlum.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class DatabaseRefactor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "unk2_1",
                table: "home_lands");

            migrationBuilder.DropColumn(
                name: "unk3_1",
                table: "home_lands");

            migrationBuilder.DropColumn(
                name: "unk4_2",
                table: "home_lands");

            migrationBuilder.RenameColumn(
                name: "time",
                table: "home_lands",
                newName: "status");

            migrationBuilder.RenameColumn(
                name: "is_most_recent",
                table: "home_lands",
                newName: "repeat");

            migrationBuilder.AlterColumn<string>(
                name: "unk1_16",
                table: "player_accounts",
                type: "varchar(32)",
                maxLength: 32,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(16)",
                oldMaxLength: 16,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<byte>(
                name: "countdown",
                table: "home_lands",
                type: "tinyint unsigned",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "heartbeat_mode",
                table: "home_lands",
                type: "tinyint unsigned",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<DateTime>(
                name: "last_update",
                table: "home_lands",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "countdown",
                table: "home_lands");

            migrationBuilder.DropColumn(
                name: "heartbeat_mode",
                table: "home_lands");

            migrationBuilder.DropColumn(
                name: "last_update",
                table: "home_lands");

            migrationBuilder.RenameColumn(
                name: "status",
                table: "home_lands",
                newName: "time");

            migrationBuilder.RenameColumn(
                name: "repeat",
                table: "home_lands",
                newName: "is_most_recent");

            migrationBuilder.AlterColumn<string>(
                name: "unk1_16",
                table: "player_accounts",
                type: "varchar(16)",
                maxLength: 16,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(32)",
                oldMaxLength: 32,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<byte>(
                name: "unk2_1",
                table: "home_lands",
                type: "tinyint unsigned",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "unk3_1",
                table: "home_lands",
                type: "tinyint unsigned",
                nullable: true);

            migrationBuilder.AddColumn<ushort>(
                name: "unk4_2",
                table: "home_lands",
                type: "smallint unsigned",
                nullable: true);
        }
    }
}
