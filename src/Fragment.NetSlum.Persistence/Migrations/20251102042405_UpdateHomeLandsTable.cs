using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fragment.NetSlum.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateHomeLandsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_player_accounts_save_id",
                table: "player_accounts");

            migrationBuilder.AddColumn<DateTime>(
                name: "last_login",
                table: "player_accounts",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "password_hash",
                table: "player_accounts",
                type: "varchar(60)",
                maxLength: 60,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "unk1_16",
                table: "player_accounts",
                type: "varchar(16)",
                maxLength: 16,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<byte>(
                name: "unk2_1",
                table: "player_accounts",
                type: "tinyint unsigned",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "unk3_1",
                table: "player_accounts",
                type: "tinyint unsigned",
                nullable: true);

            migrationBuilder.AlterColumn<byte>(
                name: "registered_player_cnt",
                table: "home_lands",
                type: "tinyint unsigned",
                nullable: false,
                oldClrType: typeof(sbyte),
                oldType: "tinyint");

            migrationBuilder.AlterColumn<byte>(
                name: "max_player_cnt",
                table: "home_lands",
                type: "tinyint unsigned",
                nullable: false,
                oldClrType: typeof(sbyte),
                oldType: "tinyint");

            migrationBuilder.AddColumn<int>(
                name: "player_account_id",
                table: "home_lands",
                type: "int",
                nullable: false,
                defaultValue: 0);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "last_login",
                table: "player_accounts");

            migrationBuilder.DropColumn(
                name: "password_hash",
                table: "player_accounts");

            migrationBuilder.DropColumn(
                name: "unk1_16",
                table: "player_accounts");

            migrationBuilder.DropColumn(
                name: "unk2_1",
                table: "player_accounts");

            migrationBuilder.DropColumn(
                name: "unk3_1",
                table: "player_accounts");

            migrationBuilder.DropColumn(
                name: "player_account_id",
                table: "home_lands");

            migrationBuilder.DropColumn(
                name: "unk2_1",
                table: "home_lands");

            migrationBuilder.DropColumn(
                name: "unk3_1",
                table: "home_lands");

            migrationBuilder.DropColumn(
                name: "unk4_2",
                table: "home_lands");

            migrationBuilder.AlterColumn<sbyte>(
                name: "registered_player_cnt",
                table: "home_lands",
                type: "tinyint",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint unsigned");

            migrationBuilder.AlterColumn<sbyte>(
                name: "max_player_cnt",
                table: "home_lands",
                type: "tinyint",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint unsigned");

            migrationBuilder.CreateIndex(
                name: "ix_player_accounts_save_id",
                table: "player_accounts",
                column: "save_id",
                unique: true);
        }
    }
}
