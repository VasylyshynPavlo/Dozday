using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dozday.Data.Migrations
{
    /// <inheritdoc />
    public partial class QualityAssuranceTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "QualityAssurances",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Tittle = table.Column<string>(type: "text", nullable: false),
                    AssuranceMarkdown = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QualityAssurances", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QualityAssurances");
        }
    }
}
