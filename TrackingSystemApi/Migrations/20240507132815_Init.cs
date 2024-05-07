using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrackingSystem.Api.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UGroup",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UGroup", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ULesson",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ULesson", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UPlace",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UPlace", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "URole",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_URole", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UUser",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Login = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    GroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UUser", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UUser_UGroup_GroupId",
                        column: x => x.GroupId,
                        principalTable: "UGroup",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "USubject",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Week = table.Column<int>(type: "int", nullable: false),
                    Day = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Pair = table.Column<int>(type: "int", nullable: false),
                    IsDifference = table.Column<int>(type: "int", nullable: false),
                    GroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlaceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LessonId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TeacherId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USubject", x => x.Id);
                    table.ForeignKey(
                        name: "FK_USubject_UGroup_GroupId",
                        column: x => x.GroupId,
                        principalTable: "UGroup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_USubject_ULesson_LessonId",
                        column: x => x.LessonId,
                        principalTable: "ULesson",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_USubject_UPlace_PlaceId",
                        column: x => x.PlaceId,
                        principalTable: "UPlace",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_USubject_UUser_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "UUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UUserRole",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UUserRole", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UUserRole_URole_RoleId",
                        column: x => x.RoleId,
                        principalTable: "URole",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UUserRole_UUser_UserId",
                        column: x => x.UserId,
                        principalTable: "UUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UUserSubject",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsMarked = table.Column<bool>(type: "bit", nullable: false),
                    MarkTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UUserSubject", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UUserSubject_USubject_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "USubject",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UUserSubject_UUser_UserId",
                        column: x => x.UserId,
                        principalTable: "UUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_USubject_GroupId",
                table: "USubject",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_USubject_LessonId",
                table: "USubject",
                column: "LessonId");

            migrationBuilder.CreateIndex(
                name: "IX_USubject_PlaceId",
                table: "USubject",
                column: "PlaceId");

            migrationBuilder.CreateIndex(
                name: "IX_USubject_TeacherId",
                table: "USubject",
                column: "TeacherId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UUser_GroupId",
                table: "UUser",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_UUserRole_RoleId",
                table: "UUserRole",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UUserRole_UserId",
                table: "UUserRole",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UUserSubject_SubjectId",
                table: "UUserSubject",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_UUserSubject_UserId",
                table: "UUserSubject",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UUserRole");

            migrationBuilder.DropTable(
                name: "UUserSubject");

            migrationBuilder.DropTable(
                name: "URole");

            migrationBuilder.DropTable(
                name: "USubject");

            migrationBuilder.DropTable(
                name: "ULesson");

            migrationBuilder.DropTable(
                name: "UPlace");

            migrationBuilder.DropTable(
                name: "UUser");

            migrationBuilder.DropTable(
                name: "UGroup");
        }
    }
}
