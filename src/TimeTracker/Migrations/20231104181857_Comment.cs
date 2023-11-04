using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Xan.TimeTracker.Migrations
{
    /// <inheritdoc />
    public partial class Comment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsReviewed",
                table: "Entries");

            migrationBuilder.AddColumn<string>(
                name: "Comment",
                table: "Entries",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Comment",
                table: "Entries");

            migrationBuilder.AddColumn<bool>(
                name: "IsReviewed",
                table: "Entries",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }
    }
}
