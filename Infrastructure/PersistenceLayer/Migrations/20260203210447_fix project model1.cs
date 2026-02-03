using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersistenceLayer.Migrations
{
    /// <inheritdoc />
    public partial class fixprojectmodel1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Assistants_AssistantId",
                table: "Tasks");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Doctors_DoctorId",
                table: "Tasks");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Students_StudentId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_AssistantId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_DoctorId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_StudentId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "AssistantId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "DoctorId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "StudentId",
                table: "Tasks");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AssistantId",
                table: "Tasks",
                type: "nvarchar(255)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DoctorId",
                table: "Tasks",
                type: "nvarchar(255)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StudentId",
                table: "Tasks",
                type: "nvarchar(255)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_AssistantId",
                table: "Tasks",
                column: "AssistantId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_DoctorId",
                table: "Tasks",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_StudentId",
                table: "Tasks",
                column: "StudentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Assistants_AssistantId",
                table: "Tasks",
                column: "AssistantId",
                principalTable: "Assistants",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Doctors_DoctorId",
                table: "Tasks",
                column: "DoctorId",
                principalTable: "Doctors",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Students_StudentId",
                table: "Tasks",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "Id");
        }
    }
}
