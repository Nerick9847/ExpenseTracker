using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExpenseTracker.Models;

namespace ExpenseTracker.Services
{
    public class DebtService
    {
        public DebtService() { }

        // Retrieves all debts from the database
        public async Task<List<Debt>> GetDebtsAsync()
        {
            var database = await DebtDatabase.LoadDatabaseAsync();
            return database.Debts;
        }

        // Adds a new debt to the database
        public async Task AddDebtAsync(Debt debt)
        {
            var database = await DebtDatabase.LoadDatabaseAsync();
            debt.WhenAdded = DateTime.Now; // Set the "When Added" date when the debt is added
            database.Debts.Add(debt);
            await DebtDatabase.SaveDatabaseAsync(database); // Save the updated database
        }

        // Updates an existing debt in the database
        public async Task UpdateDebtAsync(Debt updatedDebt)
        {
            var database = await DebtDatabase.LoadDatabaseAsync();
            var debt = database.Debts.FirstOrDefault(d => d.DebtId == updatedDebt.DebtId);
            if (debt != null)
            {
                // Update debt properties
                debt.DebtDescription = updatedDebt.DebtDescription;
                debt.DebtType = updatedDebt.DebtType;
                debt.DebtAmount = updatedDebt.DebtAmount;
                debt.DebtCategory = updatedDebt.DebtCategory;
                debt.DueDate = updatedDebt.DueDate;
                debt.IsPaid = updatedDebt.IsPaid;
                debt.WhenAdded = updatedDebt.WhenAdded;
            }
            await DebtDatabase.SaveDatabaseAsync(database); // Save the updated database
        }

        // Deletes a debt from the database using its unique ID
        public async Task DeleteDebtAsync(Guid debtId)
        {
            var database = await DebtDatabase.LoadDatabaseAsync();
            database.Debts.RemoveAll(d => d.DebtId == debtId); // Remove the debt matching the given ID
            await DebtDatabase.SaveDatabaseAsync(database); // Save the updated database
        }

        // Retrieves all categories from the database
        public async Task<List<string>> GetCategoriesAsync()
        {
            var database = await DebtDatabase.LoadDatabaseAsync();
            return database.Categories;
        }

        // Adds a new category to the database if it doesn't already exist
        public async Task AddCategoryAsync(string category)
        {
            var database = await DebtDatabase.LoadDatabaseAsync();
            if (!database.Categories.Contains(category))
            {
                database.Categories.Add(category); // Add new category
                await DebtDatabase.SaveDatabaseAsync(database); // Save the updated database
            }
        }

        // Calculates and returns the total unpaid debt
        public async Task<decimal> GetTotalUnpaidDebtAsync()
        {
            var database = await DebtDatabase.LoadDatabaseAsync();
            return database.Debts.Where(d => !d.IsPaid).Sum(d => d.DebtAmount); // Sum of all unpaid debts
        }
    }
}
