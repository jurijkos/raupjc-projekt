using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace WebShop.Data.Migrations
{
    public partial class ItemAdded4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "AspUserId",
                table: "Item",
                nullable: true,
                oldClrType: typeof(int));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "AspUserId",
                table: "Item",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
