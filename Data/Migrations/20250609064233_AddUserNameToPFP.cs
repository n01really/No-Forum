using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace No_Forum.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUserNameToPFP : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "PFPs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserName",
                table: "PFPs");
        }
    }
}
