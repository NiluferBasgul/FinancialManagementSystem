using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinancialManagementSystem.Core.Migrations
{
    public partial class AddedNewBudgetCategoryEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Transactions_Date",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_UserId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "Needs",
                table: "Budgets");

            migrationBuilder.DropColumn(
                name: "Savings",
                table: "Budgets");

            migrationBuilder.DropColumn(
                name: "Wants",
                table: "Budgets");

            migrationBuilder.CreateTable(
                name: "BudgetCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Value = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    BudgetId = table.Column<int>(type: "int", nullable: false),
                    BudgetId1 = table.Column<int>(type: "int", nullable: true),
                    BudgetId2 = table.Column<int>(type: "int", nullable: true),
                    BudgetId3 = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BudgetCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BudgetCategories_Budgets_BudgetId1",
                        column: x => x.BudgetId1,
                        principalTable: "Budgets",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BudgetCategories_Budgets_BudgetId2",
                        column: x => x.BudgetId2,
                        principalTable: "Budgets",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BudgetCategories_Budgets_BudgetId3",
                        column: x => x.BudgetId3,
                        principalTable: "Budgets",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BudgetCategory_Budget",
                        column: x => x.BudgetId,
                        principalTable: "Budgets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BudgetCategories_BudgetId",
                table: "BudgetCategories",
                column: "BudgetId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetCategories_BudgetId1",
                table: "BudgetCategories",
                column: "BudgetId1");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetCategories_BudgetId2",
                table: "BudgetCategories",
                column: "BudgetId2");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetCategories_BudgetId3",
                table: "BudgetCategories",
                column: "BudgetId3");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BudgetCategories");

            migrationBuilder.AddColumn<decimal>(
                name: "Needs",
                table: "Budgets",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Savings",
                table: "Budgets",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Wants",
                table: "Budgets",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_Date",
                table: "Transactions",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_UserId",
                table: "Transactions",
                column: "UserId");
        }
    }
}
