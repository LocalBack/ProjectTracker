using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectTracker.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCostToWorkLog1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Cost",
                table: "WorkLogs",
                type: "decimal(18,2)", // Genellikle parasal veriler için kullanılır
                nullable: false,
                defaultValue: 0m  // Eski kayıtlara 0 değeri atanır
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Cost",
                table: "WorkLogs"
            );
        }
    }
}
