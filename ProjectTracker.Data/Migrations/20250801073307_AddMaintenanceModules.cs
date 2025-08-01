using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable
namespace ProjectTracker.Data.Migrations
{
    public partial class AddMaintenanceModules : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            /* ---------- (1) Equipments ---------- */
            migrationBuilder.CreateTable(
                name: "Equipments",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                                         .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),

                    Name = table.Column<string>(maxLength: 200, nullable: false),
                    SerialNumber = table.Column<string>(maxLength: 100, nullable: false),
                    Type = table.Column<string>(maxLength: 100, nullable: false),

                    ProjectId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Equipments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Equipments_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Equipments_ProjectId",
                table: "Equipments",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Equipments_SerialNumber",
                table: "Equipments",
                column: "SerialNumber",
                unique: true);

            /* ---------- (2) MaintenanceSchedules ---------- */
            migrationBuilder.CreateTable(
                name: "MaintenanceSchedules",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                                                .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),

                    EquipmentId = table.Column<int>(nullable: false),
                    MaintenanceType = table.Column<string>(maxLength: 100, nullable: false),
                    IntervalDays = table.Column<int>(nullable: false),
                    LastMaintenanceDate = table.Column<DateTime>(nullable: false),
                    NextMaintenanceDate = table.Column<DateTime>(nullable: false),
                    Instructions = table.Column<string>(maxLength: 2000, nullable: true),
                    IsNotificationSent = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaintenanceSchedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MaintenanceSchedules_Equipments_EquipmentId",
                        column: x => x.EquipmentId,
                        principalTable: "Equipments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceSchedules_EquipmentId",
                table: "MaintenanceSchedules",
                column: "EquipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceSchedules_NextMaintenanceDate",
                table: "MaintenanceSchedules",
                column: "NextMaintenanceDate");

            /* ---------- (3) MaintenanceLogs ---------- */
            migrationBuilder.CreateTable(
                name: "MaintenanceLogs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                                                 .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),

                    MaintenanceScheduleId = table.Column<int>(nullable: false),
                    MaintenanceDate = table.Column<DateTime>(nullable: false),
                    PerformedBy = table.Column<string>(maxLength: 100, nullable: false),
                    Notes = table.Column<string>(maxLength: 2000, nullable: false),
                    Cost = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    IsCompleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaintenanceLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MaintenanceLogs_MaintenanceSchedules_MaintenanceScheduleId",
                        column: x => x.MaintenanceScheduleId,
                        principalTable: "MaintenanceSchedules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceLogs_MaintenanceScheduleId",
                table: "MaintenanceLogs",
                column: "MaintenanceScheduleId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "MaintenanceLogs");
            migrationBuilder.DropTable(name: "MaintenanceSchedules");
            migrationBuilder.DropTable(name: "Equipments");
        }
    }
}
