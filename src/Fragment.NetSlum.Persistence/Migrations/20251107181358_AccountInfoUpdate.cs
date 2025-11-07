using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fragment.NetSlum.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AccountInfoUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "unk2_1",
                table: "player_accounts");

            migrationBuilder.DropColumn(
                name: "unk3_1",
                table: "player_accounts");

            migrationBuilder.AddColumn<byte>(
                name: "client_type",
                table: "player_accounts",
                type: "tinyint unsigned",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "game_version",
                table: "player_accounts",
                type: "tinyint unsigned",
                nullable: false,
                defaultValue: (byte)0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "client_type",
                table: "player_accounts");

            migrationBuilder.DropColumn(
                name: "game_version",
                table: "player_accounts");

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
        }
    }
}
