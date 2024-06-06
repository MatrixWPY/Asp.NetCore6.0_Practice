using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebMVC.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Tbl_ContactInfo",
                columns: table => new
                {
                    ContactInfoID = table.Column<long>(type: "BIGINT", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "NVARCHAR(10)", maxLength: 10, nullable: false),
                    Nickname = table.Column<string>(type: "NVARCHAR(10)", maxLength: 10, nullable: true),
                    Gender = table.Column<byte>(type: "TINYINT", nullable: true),
                    Age = table.Column<byte>(type: "TINYINT", nullable: true),
                    PhoneNo = table.Column<string>(type: "VARCHAR(20)", maxLength: 20, nullable: false),
                    Address = table.Column<string>(type: "NVARCHAR(100)", maxLength: 100, nullable: false),
                    IsEnable = table.Column<bool>(type: "BIT", nullable: false, defaultValue: true),
                    CreateTime = table.Column<DateTime>(type: "DATETIME", nullable: false, defaultValueSql: "GETDATE()"),
                    UpdateTime = table.Column<DateTime>(type: "DATETIME", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tbl_ContactInfo", x => x.ContactInfoID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tbl_ContactInfo");
        }
    }
}
