using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjetoTasks.Migrations
{
    /// <inheritdoc />
    public partial class create_task_relations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ToDoTasks_Users_UserId",
                table: "ToDoTasks");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "ToDoTasks",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ToDoTasks_Users_UserId",
                table: "ToDoTasks",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ToDoTasks_Users_UserId",
                table: "ToDoTasks");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "ToDoTasks",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_ToDoTasks_Users_UserId",
                table: "ToDoTasks",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
