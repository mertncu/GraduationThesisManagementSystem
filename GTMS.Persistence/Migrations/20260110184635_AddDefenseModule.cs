using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GTMS.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddDefenseModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DefenseSessions_Committees_CommitteeId",
                table: "DefenseSessions");

            migrationBuilder.DropForeignKey(
                name: "FK_DefenseSessions_DefenseStatuses_DefenseStatusId",
                table: "DefenseSessions");

            migrationBuilder.DropForeignKey(
                name: "FK_DefenseSessions_ThesisProjects_ThesisId",
                table: "DefenseSessions");

            migrationBuilder.DropTable(
                name: "DefenseStatuses");

            migrationBuilder.DropIndex(
                name: "IX_DefenseSessions_CommitteeId",
                table: "DefenseSessions");

            migrationBuilder.DropColumn(
                name: "ActualEndAt",
                table: "DefenseSessions");

            migrationBuilder.DropColumn(
                name: "ActualStartAt",
                table: "DefenseSessions");

            migrationBuilder.DropColumn(
                name: "CommitteeId",
                table: "DefenseSessions");

            migrationBuilder.DropColumn(
                name: "IsOnline",
                table: "DefenseSessions");

            migrationBuilder.DropColumn(
                name: "OnlineMeetingLink",
                table: "DefenseSessions");

            migrationBuilder.DropColumn(
                name: "Room",
                table: "DefenseSessions");

            migrationBuilder.DropColumn(
                name: "ScheduledAt",
                table: "DefenseSessions");

            migrationBuilder.RenameColumn(
                name: "DefenseStatusId",
                table: "DefenseSessions",
                newName: "DefenseEventId");

            migrationBuilder.RenameIndex(
                name: "IX_DefenseSessions_DefenseStatusId",
                table: "DefenseSessions",
                newName: "IX_DefenseSessions_DefenseEventId");

            migrationBuilder.AddColumn<string>(
                name: "Comment",
                table: "DefenseSessions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "EndTime",
                table: "DefenseSessions",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<double>(
                name: "PresentationScore",
                table: "DefenseSessions",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "QAScore",
                table: "DefenseSessions",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "QualityScore",
                table: "DefenseSessions",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Result",
                table: "DefenseSessions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "StartTime",
                table: "DefenseSessions",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<double>(
                name: "TotalScore",
                table: "DefenseSessions",
                type: "float",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DefenseEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TermId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    SlotDurationMinutes = table.Column<int>(type: "int", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DefenseEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DefenseEvents_AcademicTerms_TermId",
                        column: x => x.TermId,
                        principalTable: "AcademicTerms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DefenseJuries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DefenseSessionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AdvisorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ExternalName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExternalInstitution = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExternalEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsChair = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DefenseJuries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DefenseJuries_Advisors_AdvisorId",
                        column: x => x.AdvisorId,
                        principalTable: "Advisors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DefenseJuries_DefenseSessions_DefenseSessionId",
                        column: x => x.DefenseSessionId,
                        principalTable: "DefenseSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DefenseEvents_TermId",
                table: "DefenseEvents",
                column: "TermId");

            migrationBuilder.CreateIndex(
                name: "IX_DefenseJuries_AdvisorId",
                table: "DefenseJuries",
                column: "AdvisorId");

            migrationBuilder.CreateIndex(
                name: "IX_DefenseJuries_DefenseSessionId",
                table: "DefenseJuries",
                column: "DefenseSessionId");

            migrationBuilder.AddForeignKey(
                name: "FK_DefenseSessions_DefenseEvents_DefenseEventId",
                table: "DefenseSessions",
                column: "DefenseEventId",
                principalTable: "DefenseEvents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DefenseSessions_ThesisProjects_ThesisId",
                table: "DefenseSessions",
                column: "ThesisId",
                principalTable: "ThesisProjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DefenseSessions_DefenseEvents_DefenseEventId",
                table: "DefenseSessions");

            migrationBuilder.DropForeignKey(
                name: "FK_DefenseSessions_ThesisProjects_ThesisId",
                table: "DefenseSessions");

            migrationBuilder.DropTable(
                name: "DefenseEvents");

            migrationBuilder.DropTable(
                name: "DefenseJuries");

            migrationBuilder.DropColumn(
                name: "Comment",
                table: "DefenseSessions");

            migrationBuilder.DropColumn(
                name: "EndTime",
                table: "DefenseSessions");

            migrationBuilder.DropColumn(
                name: "PresentationScore",
                table: "DefenseSessions");

            migrationBuilder.DropColumn(
                name: "QAScore",
                table: "DefenseSessions");

            migrationBuilder.DropColumn(
                name: "QualityScore",
                table: "DefenseSessions");

            migrationBuilder.DropColumn(
                name: "Result",
                table: "DefenseSessions");

            migrationBuilder.DropColumn(
                name: "StartTime",
                table: "DefenseSessions");

            migrationBuilder.DropColumn(
                name: "TotalScore",
                table: "DefenseSessions");

            migrationBuilder.RenameColumn(
                name: "DefenseEventId",
                table: "DefenseSessions",
                newName: "DefenseStatusId");

            migrationBuilder.RenameIndex(
                name: "IX_DefenseSessions_DefenseEventId",
                table: "DefenseSessions",
                newName: "IX_DefenseSessions_DefenseStatusId");

            migrationBuilder.AddColumn<DateTime>(
                name: "ActualEndAt",
                table: "DefenseSessions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ActualStartAt",
                table: "DefenseSessions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CommitteeId",
                table: "DefenseSessions",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<bool>(
                name: "IsOnline",
                table: "DefenseSessions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "OnlineMeetingLink",
                table: "DefenseSessions",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Room",
                table: "DefenseSessions",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "ScheduledAt",
                table: "DefenseSessions",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "DefenseStatuses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DefenseStatuses", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DefenseSessions_CommitteeId",
                table: "DefenseSessions",
                column: "CommitteeId");

            migrationBuilder.AddForeignKey(
                name: "FK_DefenseSessions_Committees_CommitteeId",
                table: "DefenseSessions",
                column: "CommitteeId",
                principalTable: "Committees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DefenseSessions_DefenseStatuses_DefenseStatusId",
                table: "DefenseSessions",
                column: "DefenseStatusId",
                principalTable: "DefenseStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DefenseSessions_ThesisProjects_ThesisId",
                table: "DefenseSessions",
                column: "ThesisId",
                principalTable: "ThesisProjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
