using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace No_Forum.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddForumPageTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Forumpages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Political = table.Column<bool>(type: "bit", nullable: false),
                    NSFW = table.Column<bool>(type: "bit", nullable: false),
                    Roleplay = table.Column<bool>(type: "bit", nullable: false),
                    Discussion = table.Column<bool>(type: "bit", nullable: false),
                    Meme = table.Column<bool>(type: "bit", nullable: false),
                    Art = table.Column<bool>(type: "bit", nullable: false),
                    Technology = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Forumpages", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Forumpages");
        }
    }
}
