using MudBlazor;

namespace ExpenseTracker.Components.Pages
{
    public partial class Transaction
    {
        private DatabaseModel database = new();
        private FinancialTransaction transactionModel = new();
        private bool showBalanceError = false;
        private string balanceErrorMessage = string.Empty;
        private string newCategory = string.Empty;
        private bool showTransactionModal = false;
        private bool showCategoryModal = false;
        private string modalTitle = "Add Transaction";
        private string selectedType = string.Empty;
        private string selectedCategory = string.Empty;
        private string descriptionFilter = string.Empty;
        private DateTime? startDate = null;
        private DateTime? endDate = null;
        private List<FinancialTransaction> filteredTransactions = new();
        private bool isAmountSortedAscending = true;
        private bool isDateSortedAscending = true;

        protected override async Task OnInitializedAsync()
        {
            database = await TransactionDatabase.LoadDatabaseAsync();
            filteredTransactions = database.Transactions;
            BalanceState.OnBalanceChange += StateHasChanged;
        }

        private void OpenAddModal()
        {
            transactionModel = new FinancialTransaction { Date = DateTime.Now };
            modalTitle = "Add Transaction";
            showTransactionModal = true;
        }

        private void OpenEditModal(FinancialTransaction transaction)
        {
            transactionModel = new FinancialTransaction
            {
                Id = transaction.Id,
                Description = transaction.Description,
                Notes = transaction.Notes,
                Date = transaction.Date,
                Amount = transaction.Amount,
                Type = transaction.Type,
                Category = transaction.Category
            };
            modalTitle = "Edit Transaction";
            showTransactionModal = true;
        }

        private void CloseTransactionModal()
        {
            showTransactionModal = false;
            showBalanceError = false;
        }

        private async Task SaveTransaction()
        {
            // Check for expense exceeding the balance
            if (transactionModel.Type == "Expense" && transactionModel.Amount > BalanceState.CurrentBalance)
            {
                showBalanceError = true; // Enable the error flag
                balanceErrorMessage = "Expense amount exceeds the available balance.";
                StateHasChanged(); // Ensure the UI updates
                return; // Stop further execution
            }

            if (modalTitle == "Add Transaction")
            {
                transactionModel.Id = Guid.NewGuid();
                database.Transactions.Add(transactionModel);
            }
            else
            {
                var existingTransaction = database.Transactions.FirstOrDefault(t => t.Id == transactionModel.Id);
                if (existingTransaction != null)
                {
                    existingTransaction.Description = transactionModel.Description;
                    existingTransaction.Notes = transactionModel.Notes;
                    existingTransaction.Date = transactionModel.Date;
                    existingTransaction.Amount = transactionModel.Amount;
                    existingTransaction.Type = transactionModel.Type;
                    existingTransaction.Category = transactionModel.Category;
                }
            }

            await TransactionDatabase.SaveDatabaseAsync(database);
            FilterTransactions();
            CloseTransactionModal();
        }

        private void FilterTransactions()
        {
            filteredTransactions = database.Transactions
                .Where(t =>
                    (string.IsNullOrEmpty(descriptionFilter) || t.Description.Contains(descriptionFilter, StringComparison.OrdinalIgnoreCase)) &&
                    (string.IsNullOrEmpty(selectedType) || t.Type == selectedType) &&
                    (string.IsNullOrEmpty(selectedCategory) || t.Category == selectedCategory) &&
                    (!startDate.HasValue || t.Date >= startDate) &&
                    (!endDate.HasValue || t.Date <= endDate))
                .ToList();
        }

        private void ClearFilters()
        {
            descriptionFilter = string.Empty;
            selectedType = string.Empty;
            selectedCategory = string.Empty;
            startDate = null;
            endDate = null;
            filteredTransactions = database.Transactions;
        }

        private void ToggleAmountSort()
        {
            isAmountSortedAscending = !isAmountSortedAscending;
            filteredTransactions = isAmountSortedAscending
                ? filteredTransactions.OrderBy(t => t.Amount).ToList()
                : filteredTransactions.OrderByDescending(t => t.Amount).ToList();
        }

        private void ToggleDateSort()
        {
            isDateSortedAscending = !isDateSortedAscending;
            filteredTransactions = isDateSortedAscending
                ? filteredTransactions.OrderBy(t => t.Date).ToList()
                : filteredTransactions.OrderByDescending(t => t.Date).ToList();
        }
        private void OpenCategoryModal()
        {
            showCategoryModal = true;
        }

        private void CloseCategoryModal()
        {
            showCategoryModal = false;
        }

        private async Task AddCategory()
        {
            if (!string.IsNullOrEmpty(newCategory) && !database.Categories.Contains(newCategory))
            {
                database.Categories.Add(newCategory);
                await TransactionDatabase.SaveDatabaseAsync(database);
            }
            newCategory = string.Empty;
            CloseCategoryModal();
        }

        private void DeleteTransaction(FinancialTransaction transaction)
        {
            database.Transactions.Remove(transaction);
            TransactionDatabase.SaveDatabaseAsync(database);
            FilterTransactions();
        }

        private void ExportTransactions()
        {
            var transactionsToExport = filteredTransactions.Select(t => new
            {
                t.Description,
                t.Notes,
                Date = t.Date.ToString("yyyy-MM-dd"),
                Amount = t.Amount.ToString("C"),
                t.Type,
                t.Category
            }).ToList();
        }
    }
}