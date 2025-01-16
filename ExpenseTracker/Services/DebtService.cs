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

        public async Task<List<Debt>> GetDebtsAsync()
        {
            var database = await DebtDatabase.LoadDatabaseAsync();
            return database.Debts;
        }

        public async Task AddDebtAsync(Debt debt)
        {
            var database = await DebtDatabase.LoadDatabaseAsync();
            debt.WhenAdded = DateTime.Now; // Set the "When Added" date when the debt is added
            database.Debts.Add(debt);
            await DebtDatabase.SaveDatabaseAsync(database);
        }

        public async Task UpdateDebtAsync(Debt updatedDebt)
        {
            var database = await DebtDatabase.LoadDatabaseAsync();
            var debt = database.Debts.FirstOrDefault(d => d.DebtId == updatedDebt.DebtId);
            if (debt != null)
            {
                debt.DebtDescription = updatedDebt.DebtDescription;
                debt.DebtType = updatedDebt.DebtType;
                debt.DebtAmount = updatedDebt.DebtAmount;
                debt.DebtCategory = updatedDebt.DebtCategory;
                debt.DueDate = updatedDebt.DueDate;
                debt.IsPaid = updatedDebt.IsPaid;
                debt.WhenAdded = updatedDebt.WhenAdded; 
            }
            await DebtDatabase.SaveDatabaseAsync(database);
        }

        public async Task DeleteDebtAsync(Guid debtId)
        {
            var database = await DebtDatabase.LoadDatabaseAsync();
            database.Debts.RemoveAll(d => d.DebtId == debtId);
            await DebtDatabase.SaveDatabaseAsync(database);
        }

        public async Task<List<string>> GetCategoriesAsync()
        {
            var database = await DebtDatabase.LoadDatabaseAsync();
            return database.Categories;
        }

        public async Task AddCategoryAsync(string category)
        {
            var database = await DebtDatabase.LoadDatabaseAsync();
            if (!database.Categories.Contains(category))
            {
                database.Categories.Add(category);
                await DebtDatabase.SaveDatabaseAsync(database);
            }
        }

        public async Task<decimal> GetTotalUnpaidDebtAsync()
        {
            var database = await DebtDatabase.LoadDatabaseAsync();
            return database.Debts.Where(d => !d.IsPaid).Sum(d => d.DebtAmount);
        }
    }
}
