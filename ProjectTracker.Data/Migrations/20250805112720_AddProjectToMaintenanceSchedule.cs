using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectTracker.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddProjectToMaintenanceSchedule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProjectId",
                table: "MaintenanceSchedules",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceSchedules_ProjectId",
                table: "MaintenanceSchedules",
                column: "ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_MaintenanceSchedules_Projects_ProjectId",
                table: "MaintenanceSchedules",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MaintenanceSchedules_Projects_ProjectId",
                table: "MaintenanceSchedules");

            migrationBuilder.DropIndex(
                name: "IX_MaintenanceSchedules_ProjectId",
                table: "MaintenanceSchedules");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "MaintenanceSchedules");
        }
    }
}
