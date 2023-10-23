using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CDRApi.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Calls",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    caller_id = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Recipient = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    call_date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    end_time = table.Column<TimeSpan>(type: "TEXT", nullable: false),
                    Duration = table.Column<int>(type: "INTEGER", nullable: false),
                    Cost = table.Column<decimal>(type: "TEXT", precision: 18, scale: 3, nullable: false),
                    Reference = table.Column<string>(type: "TEXT", maxLength: 33, nullable: false),
                    Currency = table.Column<string>(type: "TEXT", maxLength: 3, nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Calls", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Calls");
        }
    }
}
