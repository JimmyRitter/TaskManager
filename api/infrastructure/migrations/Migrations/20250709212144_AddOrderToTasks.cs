using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SaasTaskManager.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderToTasks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "Tasks",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            // Update existing tasks with proper order values per list
            // This is idempotent - only updates tasks that still have the default order value (0)
            // and orders them by CreatedAt within each list
            migrationBuilder.Sql(@"
                WITH ordered_tasks AS (
                    SELECT 
                        ""Id"",
                        ROW_NUMBER() OVER (PARTITION BY ""ListId"" ORDER BY ""CreatedAt"") - 1 AS new_order
                    FROM ""Tasks""
                    WHERE ""Order"" = 0
                )
                UPDATE ""Tasks""
                SET ""Order"" = ordered_tasks.new_order
                FROM ordered_tasks
                WHERE ""Tasks"".""Id"" = ordered_tasks.""Id""
                AND ""Tasks"".""Order"" = 0;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Order",
                table: "Tasks");
        }
    }
}
