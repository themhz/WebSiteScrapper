using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebSiteScrapper.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           
            migrationBuilder.CreateTable(name: "Urls",
               columns: table => new
               {
                   Id = table.Column<string>(maxLength: 50, nullable: false),
                   Title = table.Column<string>(nullable: true),
                   Url = table.Column<string>(maxLength: 500, nullable: true),
                   Baseurl = table.Column<string>(maxLength: 500, nullable: true),
                   Hash = table.Column<string>(maxLength: 500, nullable: true),
                   Date = table.Column<DateTime>(maxLength: 500, nullable: true),
               });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {            
            
        }
    }
}
