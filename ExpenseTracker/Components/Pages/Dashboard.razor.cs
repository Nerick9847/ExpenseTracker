namespace ExpenseTracker.Components.Pages
{
    public partial class Dashboard
    {
        private decimal TotalIncome;
        private decimal TotalExpense;
        private decimal TotalPendingDebt;
        private decimal TotalPaidDebt;
        private decimal Balance;

        private List<FinancialTransaction> AllTransactions = new();
        private List<FinancialTransaction> InflowTransactions = new();
        private List<FinancialTransaction> OutflowTransactions = new();
        private List<ExpenseTracker.Models.Debt> AllDebts = new();
        private List<ExpenseTracker.Models.Debt> PendingDebts = new();
        private List<ExpenseTracker.Models.Debt> PaidDebts = new();

        private DateTime StartDate = DateTime.Now.AddMonths(-1);
        private DateTime EndDate = DateTime.Now;

        private double[] transactionCounts = new double[4]; // [Income, Expense, PendingDebt, PaidDebt]
        private string[] transactionLabels = { "Inflow", "Outflow", "Pending Debt", "Paid Debt" };

        protected override async Task OnInitializedAsync()
        {
            // Load all transactions and debts
            AllTransactions = await transactionService.GetTransactionsAsync();
            AllDebts = await debtService.GetDebtsAsync();

            // Display all data by default
            DisplayAllData();
        }

        private void ApplyFilter()
        {
            InflowTransactions = AllTransactions
                .Where(t => t.Type == "Income" && t.Date >= StartDate && t.Date <= EndDate)
                .ToList();

            OutflowTransactions = AllTransactions
                .Where(t => t.Type == "Expense" && t.Date >= StartDate && t.Date <= EndDate)
                .ToList();

            PendingDebts = AllDebts
                .Where(d => !d.IsPaid && d.DueDate >= StartDate && d.DueDate <= EndDate)
                .ToList();

            PaidDebts = AllDebts
                .Where(d => d.IsPaid && d.DueDate >= StartDate && d.DueDate <= EndDate)
                .ToList();

            UpdateTotalsAndChart();
        }

        private void ClearFilter()
        {
            StartDate = DateTime.Now.AddMonths(-1);
            EndDate = DateTime.Now;
            DisplayAllData();
        }

        private void DisplayAllData()
        {
            InflowTransactions = AllTransactions.Where(t => t.Type == "Income").ToList();
            OutflowTransactions = AllTransactions.Where(t => t.Type == "Expense").ToList();
            PendingDebts = AllDebts.Where(d => !d.IsPaid).ToList();
            PaidDebts = AllDebts.Where(d => d.IsPaid).ToList();

            UpdateTotalsAndChart();
        }

        private void UpdateTotalsAndChart()
        {
            TotalIncome = InflowTransactions.Sum(t => t.Amount);
            TotalExpense = OutflowTransactions.Sum(t => t.Amount);
            TotalPendingDebt = PendingDebts.Sum(d => d.DebtAmount);
            TotalPaidDebt = PaidDebts.Sum(d => d.DebtAmount);

            // Updated balance calculation: Inflow - Outflow - PaidDebt
            Balance = TotalIncome - TotalExpense - TotalPaidDebt;

            // Update pie chart data
            transactionCounts[0] = InflowTransactions.Count;
            transactionCounts[1] = OutflowTransactions.Count;
            transactionCounts[2] = PendingDebts.Count;
            transactionCounts[3] = PaidDebts.Count;

            //Update the shared balance state
            BalanceState.UpdateBalance(Balance);
        }
    }
}