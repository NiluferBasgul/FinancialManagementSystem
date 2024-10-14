using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinancialManagementSystem.Core.Migrations
{
    public partial class RemovedUnnessesaryPKs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BudgetCategory_NeedsBudget",
                table: "BudgetCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_BudgetCategory_SavingsBudget",
                table: "BudgetCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_BudgetCategory_WantsBudget",
                table: "BudgetCategories");

            migrationBuilder.AddForeignKey(
                name: "FK_BudgetCategories_Budgets_NeedsBudgetId",
                table: "BudgetCategories",
                column: "NeedsBudgetId",
                principalTable: "Budgets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BudgetCategories_Budgets_SavingsBudgetId",
                table: "BudgetCategories",
                column: "SavingsBudgetId",
                principalTable: "Budgets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BudgetCategories_Budgets_WantsBudgetId",
                table: "BudgetCategories",
                column: "WantsBudgetId",
                principalTable: "Budgets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BudgetCategories_Budgets_NeedsBudgetId",
                table: "BudgetCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_BudgetCategories_Budgets_SavingsBudgetId",
                table: "BudgetCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_BudgetCategories_Budgets_WantsBudgetId",
                table: "BudgetCategories");

            migrationBuilder.AddForeignKey(
                name: "FK_BudgetCategory_NeedsBudget",
                table: "BudgetCategories",
                column: "NeedsBudgetId",
                principalTable: "Budgets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BudgetCategory_SavingsBudget",
                table: "BudgetCategories",
                column: "SavingsBudgetId",
                principalTable: "Budgets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BudgetCategory_WantsBudget",
                table: "BudgetCategories",
                column: "WantsBudgetId",
                principalTable: "Budgets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
