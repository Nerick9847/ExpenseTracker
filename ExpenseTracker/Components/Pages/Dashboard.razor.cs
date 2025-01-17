namespace ExpenseTracker.Components.Pages
{
    public partial class Dashboard
    {   
        //Summar data for dashboard cards
        private decimal TotalIncome;
        private decimal TotalExpense;
        private decimal TotalPendingDebt;
        private decimal TotalPaidDebt;
        private decimal Balance;

        //List to hold transaction and debt data
        private List<FinancialTransaction> AllTransactions = new();
        private List<FinancialTransaction> InflowTransactions = new();
        private List<FinancialTransaction> OutflowTransactions = new();
        private List<ExpenseTracker.Models.Debt> AllDebts = new();
        private List<ExpenseTracker.Models.Debt> PendingDebts = new();
        private List<ExpenseTracker.Models.Debt> PaidDebts = new();

        //Date range for filtering transactions and debts
        private DateTime StartDate = DateTime.Now.AddMonths(-1); //Default start date is one month ago
        private DateTime EndDate = DateTime.Now; // Default end date is today

        //Data for the pie chart 
        private double[] transactionCounts = new double[4]; // [Income, Expense, PendingDebt, PaidDebt]
        private string[] transactionLabels = { "Inflow", "Outflow", "Pending Debt", "Paid Debt" };

        //Called when the component is initialized
        protected override async Task OnInitializedAsync()
        {
            try
            {
                //Load all transactions and debts from services
                AllTransactions = await transactionService.GetTransactionsAsync();
                AllDebts = await debtService.GetDebtsAsync();

                //Display all data by default
                DisplayAllData();
            }
            catch (Exception ex)
            {
                //Log or handle initialization errors
                Console.Error.WriteLine($"Error loading data: {ex.Message}");
            }
        }

        //Apply date filteration
        private void ApplyFilter()
        {
            try
            {
                // Filter income transactions within the date range
                InflowTransactions = AllTransactions
                    .Where(t => t.Type == "Income" && t.Date >= StartDate && t.Date <= EndDate)
                    .ToList();

                // Filter expense transactions within the date range
                OutflowTransactions = AllTransactions
                    .Where(t => t.Type == "Expense" && t.Date >= StartDate && t.Date <= EndDate)
                    .ToList();

                // Filter pending debts within the date range
                PendingDebts = AllDebts
                    .Where(d => !d.IsPaid && d.DueDate >= StartDate && d.DueDate <= EndDate)
                    .ToList();

                // Filter paid debts within the date range
                PaidDebts = AllDebts
                    .Where(d => d.IsPaid && d.DueDate >= StartDate && d.DueDate <= EndDate)
                    .ToList();

                // Update summary totals and chart data
                UpdateTotalsAndChart();
            }
            catch (Exception ex)
            {
                // Log or handle filter application errors
                Console.Error.WriteLine($"Error applying filter: {ex.Message}");
            }
        }

        //Clear filter and reset to degault date
        private void ClearFilter()
        {
            StartDate = DateTime.Now.AddMonths(-1); //Reset start date to one month ago
            EndDate = DateTime.Now; //Reset end date to today
            DisplayAllData();
        }

        //display all transactions and debts
        private void DisplayAllData()
        {
            InflowTransactions = AllTransactions.Where(t => t.Type == "Income").ToList();
            OutflowTransactions = AllTransactions.Where(t => t.Type == "Expense").ToList();
            PendingDebts = AllDebts.Where(d => !d.IsPaid).ToList();
            PaidDebts = AllDebts.Where(d => d.IsPaid).ToList();

            UpdateTotalsAndChart();
        }

        //Update summary and pie chart data
        private void UpdateTotalsAndChart()
        {
            //calculate total income, expense, pending debt and apid debt
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