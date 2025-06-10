using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace No_Forum.Data.Migrations
{
    /// <inheritdoc />
    public partial class Dmbutbetter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //    migrationBuilder.CreateTable(
            //        name: "DM",
            //        columns: table => new
            //        {
            //            Id = table.Column<int>(type: "int", nullable: false)
            //                .Annotation("SqlServer:Identity", "1, 1"),
            //            SenderId = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //            ReciverId = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //            Message = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //            CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
            //            SenderName = table.Column<string>(type: "nvarchar(max)", nullable: true)
            //        },
            //        constraints: table =>
            //        {
            //            table.PrimaryKey("PK_DM", x => x.Id);
            //        });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            
        }
    }
}
