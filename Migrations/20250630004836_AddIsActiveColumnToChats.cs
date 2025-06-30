﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TWeb.Migrations
{
    /// <inheritdoc />
    public partial class AddIsActiveColumnToChats : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Chats",
                type: "bit",
                nullable: false,
                defaultValue: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Chats");
        }
    }
}