using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersonalLibraryApi.Migrations
{
    /// <inheritdoc />
    public partial class statusToStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "status",
                table: "UserBooks",
                newName: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Status",
                table: "UserBooks",
                newName: "status");
        }
    }
}
