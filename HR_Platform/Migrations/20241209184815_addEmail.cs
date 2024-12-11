using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HR_Platform.Migrations
{
    /// <inheritdoc />
    public partial class addEmail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LeaveRequests_Employees_EmployeeID",
                table: "LeaveRequests");

            migrationBuilder.AlterColumn<int>(
                name: "EmployeeID",
                table: "LeaveRequests",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Employees",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_LeaveRequests_Employees_EmployeeID",
                table: "LeaveRequests",
                column: "EmployeeID",
                principalTable: "Employees",
                principalColumn: "EmployeeID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LeaveRequests_Employees_EmployeeID",
                table: "LeaveRequests");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Employees");

            migrationBuilder.AlterColumn<int>(
                name: "EmployeeID",
                table: "LeaveRequests",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_LeaveRequests_Employees_EmployeeID",
                table: "LeaveRequests",
                column: "EmployeeID",
                principalTable: "Employees",
                principalColumn: "EmployeeID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
