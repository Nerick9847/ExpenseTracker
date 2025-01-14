using System.Collections.Generic;
using System.Threading.Tasks;
using ExpenseTracker.Models;

namespace ExpenseTracker.Services
{
    public class DatabaseService
    {
        private readonly JsonDbHandler<FinancialTransaction> _transactionHandler;
        private readonly JsonDbHandler<string> _categoryHandler;

        public DatabaseService(string transactionFilePath, string categoryFilePath)
        {
            _transactionHandler = new JsonDbHandler<FinancialTransaction>(transactionFilePath);
            _categoryHandler = new JsonDbHandler<string>(categoryFilePath);
        }

        public async Task<List<FinancialTransaction>> GetTransactionsAsync() => await _transactionHandler.ReadAsync();

        public async Task SaveTransactionsAsync(List<FinancialTransaction> transactions) =>
            await _transactionHandler.WriteAsync(transactions);

        public async Task<List<string>> GetCategoriesAsync() => await _categoryHandler.ReadAsync();

        public async Task SaveCategoriesAsync(List<string> categories) =>
            await _categoryHandler.WriteAsync(categories);
    }
}
