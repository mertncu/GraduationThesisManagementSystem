using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GTMS.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RelinkSubmissionToMilestone : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Submissions_ThesisMilestones_ThesisMilestoneId",
                table: "Submissions");

            migrationBuilder.DropIndex(
                name: "IX_Submissions_ThesisMilestoneId",
                table: "Submissions");

            migrationBuilder.DropColumn(
                name: "ThesisMilestoneId",
                table: "Submissions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ThesisMilestoneId",
                table: "Submissions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Submissions_ThesisMilestoneId",
                table: "Submissions",
                column: "ThesisMilestoneId");

            migrationBuilder.AddForeignKey(
                name: "FK_Submissions_ThesisMilestones_ThesisMilestoneId",
                table: "Submissions",
                column: "ThesisMilestoneId",
                principalTable: "ThesisMilestones",
                principalColumn: "Id");
        }
    }
}
