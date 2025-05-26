using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace No_Forum.Data.Migrations
{
    /// <inheritdoc />
    public partial class comments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PostsId",
                table: "Coments",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Coments_PostsId",
                table: "Coments",
                column: "PostsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Coments_Posts_PostsId",
                table: "Coments",
                column: "PostsId",
                principalTable: "Posts",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Coments_Posts_PostsId",
                table: "Coments");

            migrationBuilder.DropIndex(
                name: "IX_Coments_PostsId",
                table: "Coments");

            migrationBuilder.DropColumn(
                name: "PostsId",
                table: "Coments");
        }
    }
}
