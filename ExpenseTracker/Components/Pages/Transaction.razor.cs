using MudBlazor;

namespace ExpenseTracker.Components.Pages
{
    public partial class Transaction
    {
        // Holds the database instance with transaction and category data
        private DatabaseModel database = new();

        // Model for storing transaction details
        private FinancialTransaction transactionModel = new();

        // Flags and messages for balance validation errors
        private bool showBalanceError = false;
        private string balanceErrorMessage = string.Empty;

        // Holds the value for a new category to be added
        private string newCategory = string.Empty;

        // Flags to control the visibility of modals
        private bool showTransactionModal = false;
        private bool showCategoryModal = false;

        // Title for the transaction modal (Add or Edit)
        private string modalTitle = "Add Transaction";

        // Filters for transaction list
        private string selectedType = string.Empty;
        private string selectedCategory = string.Empty;
        private string descriptionFilter = string.Empty;
        private DateTime? startDate = null;
        private DateTime? endDate = null;

        // Filtered list of transactions displayed to the user
        private List<FinancialTransaction> filteredTransactions = new();

        // Flags to control sorting order
        private bool isAmountSortedAscending = true;
        private bool isDateSortedAscending = true;

        // Initialization logic to load the database and set up state listeners
        protected override async Task OnInitializedAsync()
        {
            database = await TransactionDatabase.LoadDatabaseAsync(); // Load data from storage
            filteredTransactions = database.Transactions; // Initialize filtered transactions
            BalanceState.OnBalanceChange += StateHasChanged; // Refresh UI on balance changes
        }

        // Opens the modal for adding a new transaction
        private void OpenAddModal()
        {
            transactionModel = new FinancialTransaction { Date = DateTime.Now }; // Initialize model with current date
            modalTitle = "Add Transaction";
            showTransactionModal = true;
        }

        // Opens the modal for editing an existing transaction
        private void OpenEditModal(FinancialTransaction transaction)
        {
            // Clone the selected transaction data for editing
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

        // Closes the transaction modal and resets related flags
        private void CloseTransactionModal()
        {
            showTransactionModal = false;
            showBalanceError = false;
        }

        // Saves the transaction (add or edit) to the database
        private async Task SaveTransaction()
        {
            // Validate if expense exceeds available balance
            if (transactionModel.Type == "Expense" && transactionModel.Amount > BalanceState.CurrentBalance)
            {
                showBalanceError = true;
                balanceErrorMessage = "Expense amount exceeds the available balance.";
                StateHasChanged(); // Update UI
                return;
            }

            // Add a new transaction
            if (modalTitle == "Add Transaction")
            {
                transactionModel.Id = Guid.NewGuid(); // Assign a unique ID
                database.Transactions.Add(transactionModel);
            }
            else
            {
                // Update the existing transaction
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

            await TransactionDatabase.SaveDatabaseAsync(database); // Save changes to the database
            FilterTransactions(); // Apply filters to the updated list
            CloseTransactionModal(); // Close the modal
        }

        // Filters transactions based on the specified criteria
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

        // Clears all transaction filters and shows the entire list
        private void ClearFilters()
        {
            descriptionFilter = string.Empty;
            selectedType = string.Empty;
            selectedCategory = string.Empty;
            startDate = null;
            endDate = null;
            filteredTransactions = database.Transactions;
        }

        // Toggles the sorting order for transaction amounts
        private void ToggleAmountSort()
        {
            isAmountSortedAscending = !isAmountSortedAscending;
            filteredTransactions = isAmountSortedAscending
                ? filteredTransactions.OrderBy(t => t.Amount).ToList()
                : filteredTransactions.OrderByDescending(t => t.Amount).ToList();
        }

        // Toggles the sorting order for transaction dates
        private void ToggleDateSort()
        {
            isDateSortedAscending = !isDateSortedAscending;
            filteredTransactions = isDateSortedAscending
                ? filteredTransactions.OrderBy(t => t.Date).ToList()
                : filteredTransactions.OrderByDescending(t => t.Date).ToList();
        }

        // Opens the modal to add a new category
        private void OpenCategoryModal()
        {
            showCategoryModal = true;
        }

        // Closes the category modal
        private void CloseCategoryModal()
        {
            showCategoryModal = false;
        }

        // Adds a new category to the database
        private async Task AddCategory()
        {
            if (!string.IsNullOrEmpty(newCategory) && !database.Categories.Contains(newCategory))
            {
                database.Categories.Add(newCategory);
                await TransactionDatabase.SaveDatabaseAsync(database); // Save new category
            }
            newCategory = string.Empty; // Reset the input
            CloseCategoryModal();
        }

        // Deletes a specific transaction from the database
        private void DeleteTransaction(FinancialTransaction transaction)
        {
            database.Transactions.Remove(transaction); // Remove transaction
            TransactionDatabase.SaveDatabaseAsync(database); // Save changes
            FilterTransactions(); // Update the filtered list
        }

        // Exports the filtered transactions (implementation pending)
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
