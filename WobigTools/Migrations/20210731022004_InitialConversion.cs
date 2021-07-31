using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WobigTools.Migrations
{
    public partial class InitialConversion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WatcherAudits",
                columns: table => new
                {
                    WatcherAuditID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TimeStamp = table.Column<DateTime>(type: "TEXT", nullable: false),
                    TrackerID = table.Column<Guid>(type: "TEXT", nullable: false),
                    WatcherName = table.Column<string>(type: "TEXT", nullable: true),
                    ChangingUser = table.Column<string>(type: "TEXT", nullable: true),
                    State = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WatcherAudits", x => x.WatcherAuditID);
                });

            migrationBuilder.CreateTable(
                name: "WatcherEvents",
                columns: table => new
                {
                    WatcherEventID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TimeStamp = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Event = table.Column<string>(type: "TEXT", nullable: true),
                    TrackerID = table.Column<Guid>(type: "TEXT", nullable: false),
                    FriendlyName = table.Column<string>(type: "TEXT", nullable: true),
                    Triggered = table.Column<bool>(type: "INTEGER", nullable: false),
                    PageURL = table.Column<string>(type: "TEXT", nullable: true),
                    Keyword = table.Column<string>(type: "TEXT", nullable: true),
                    AlertOnKeywordNotExist = table.Column<bool>(type: "INTEGER", nullable: false),
                    Enabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    CheckInterval = table.Column<string>(type: "TEXT", nullable: true),
                    AlertDestination = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WatcherEvents", x => x.WatcherEventID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WatcherAudits");

            migrationBuilder.DropTable(
                name: "WatcherEvents");
        }
    }
}
