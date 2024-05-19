using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrackingSystem.Api.Migrations
{
    public partial class changeinfo2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<TimeSpan>(
                name: "AllowedDeviation",
                table: "Infos",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AllowedDeviation",
                table: "Infos");
        }
    }
}
