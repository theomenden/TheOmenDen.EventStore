using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TheOmenDen.EventStore.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Logging");

            migrationBuilder.CreateTable(
                name: "Aggregates",
                schema: "Logging",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newsequentialid())"),
                    Class = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExpiresAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TenantIdentifier = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Aggregate_Id", x => x.Id)
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateTable(
                name: "SerializedCommands",
                schema: "Logging",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newsequentialid())"),
                    ExpectedVersion = table.Column<int>(type: "int", nullable: true),
                    IdentityTenant = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdentityUser = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AggregateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CommandIdentifier = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CommandClass = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CommandType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CommandData = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SendScheduled = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    SendStarted = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    SendCompleted = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    SendCancelled = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    SendStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SendError = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SerializedCommand_Id", x => x.Id)
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateTable(
                name: "SerializedEvents",
                schema: "Logging",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newsequentialid())"),
                    MajorVersion = table.Column<int>(type: "int", nullable: false),
                    MinorVersion = table.Column<int>(type: "int", nullable: false),
                    IdentityTenant = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdentityUser = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EventTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UnderlyingType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EventType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EventData = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SerializedEvent_Id", x => x.Id)
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateTable(
                name: "Snapshots",
                schema: "Logging",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newsequentialid())"),
                    AggregateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AggregateMajorVersion = table.Column<int>(type: "int", nullable: false),
                    AggregateMinorVersion = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Snapshot_Id", x => x.Id)
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CommandExpectedVersion",
                schema: "Logging",
                table: "SerializedCommands",
                column: "ExpectedVersion")
                .Annotation("SqlServer:Clustered", false)
                .Annotation("SqlServer:Include", new[] { "SendStatus", "CommandData" });

            migrationBuilder.CreateIndex(
                name: "IX_EventMajorMinorVersion",
                schema: "Logging",
                table: "SerializedEvents",
                columns: new[] { "MajorVersion", "MinorVersion" })
                .Annotation("SqlServer:Clustered", false)
                .Annotation("SqlServer:Include", new[] { "EventType", "EventData" });

            migrationBuilder.CreateIndex(
                name: "IX_Snapshot_AggregateId",
                schema: "Logging",
                table: "Snapshots",
                column: "AggregateId");

            migrationBuilder.CreateIndex(
                name: "IX_Snapshot_AggregateMajorMinorVersion",
                schema: "Logging",
                table: "Snapshots",
                columns: new[] { "AggregateMajorVersion", "AggregateMinorVersion" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Aggregates",
                schema: "Logging");

            migrationBuilder.DropTable(
                name: "SerializedCommands",
                schema: "Logging");

            migrationBuilder.DropTable(
                name: "SerializedEvents",
                schema: "Logging");

            migrationBuilder.DropTable(
                name: "Snapshots",
                schema: "Logging");
        }
    }
}
