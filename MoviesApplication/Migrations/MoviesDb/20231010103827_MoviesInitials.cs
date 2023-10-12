using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MoviesApplication.Migrations.MoviesDb
{
    public partial class MoviesInitials : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MoviesShows",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Genre = table.Column<int>(type: "int", nullable: false),
                    Rating = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    ImdbUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MoviesShows", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MoviesShows");
        }
    }
}
