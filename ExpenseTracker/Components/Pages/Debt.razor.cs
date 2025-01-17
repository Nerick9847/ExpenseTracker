using ExpenseTracker.Services;
using MudBlazor;

namespace ExpenseTracker.Components.Pages
{
    //Main component to manage debts
    public partial class Debt
    {
        //List to store all debts and filered debts
        private List<ExpenseTracker.Models.Debt> debts = new();
        private List<ExpenseTracker.Models.Debt> filteredDebts = new();
       
        //Current debt being added or edited
        private ExpenseTracker.Models.Debt currentDebt = new();
        
        //Variables for handeling balance errors, modal visibility and sorting
        private bool showBalanceError = false;
        private string balanceErrorMessage = string.Empty;
        private bool isModalVisible = false;
        private bool isEditMode = false;

        //Filter variables to serach debts
        private string filterDescription = "";
        private string filterCategory = "";
        private string _selectedPaymentStatus = "";

        //Property for selectionn payment status filter i.e. "Paid" or "Pending"
        private string selectedPaymentStatus
        {
            get => _selectedPaymentStatus;
            set
            {
                _selectedPaymentStatus = value;
                filterIsPaid = value switch
                {
                    "Paid" => true,
                    "Pending" => false,
                    _ => null
                };
                ApplyFilter();
            }
        }
        private bool? filterIsPaid = null; //To track if debts are paid or pending
        private DateTime? startDate = null;
        private DateTime? endDate = null;

        private List<string> categories = new List<string> { "Personal", "Business", "Student Loan", "Home Loan" };
        private bool isSortedAscending = true;

        private decimal currentBalance = 1000m; // Example balance

        //Called when the component is initialized to load debts and appy filters
        protected override async Task OnInitializedAsync()
        {
            await LoadDebtsAsync(); //load debts from the service
            ApplyFilter(); //Apply any filters that are selected
        }

        //Method to load debts from the DebtService
        private async Task LoadDebtsAsync()
        {
            try
            {
                debts = await DebtService.GetDebtsAsync();
            }
            catch (Exception)
            {
                debts = new List<ExpenseTracker.Models.Debt>();
            }
        }

        //Method to apply filters to the list of debts
        private void ApplyFilter()
        {
            var query = debts.AsQueryable(); //conver debts list to queryable

            //Filter by description if a serach inupt is given
            if (!string.IsNullOrWhiteSpace(filterDescription))
            {
                query = query.Where(d => d.DebtDescription != null &&
                    d.DebtDescription.Contains(filterDescription, StringComparison.OrdinalIgnoreCase));
            }

            // Filter by selected category
            if (!string.IsNullOrWhiteSpace(filterCategory))
            {
                query = query.Where(d => d.DebtCategory != null &&
                    d.DebtCategory.Equals(filterCategory, StringComparison.OrdinalIgnoreCase));
            }

            //Filter by payment status i.e. Paid pr Pending
            if (filterIsPaid.HasValue)
            {
                query = query.Where(d => d.IsPaid == filterIsPaid.Value);
            }

            //Filter by date range
            if (startDate.HasValue && endDate.HasValue)
            {
                var endDateAdjusted = endDate.Value.Date.AddDays(1).AddSeconds(-1);
                query = query.Where(d => d.DueDate.Date >= startDate.Value.Date &&
                                       d.DueDate.Date <= endDateAdjusted.Date);
            }
            else if (startDate.HasValue)
            {
                query = query.Where(d => d.DueDate.Date >= startDate.Value.Date);
            }
            else if (endDate.HasValue)
            {
                var endDateAdjusted = endDate.Value.Date.AddDays(1).AddSeconds(-1);
                query = query.Where(d => d.DueDate.Date <= endDateAdjusted.Date);
            }

            //Sort the filtered debts based on debt amount i.e. ascending or descending
            filteredDebts = isSortedAscending
                ? query.OrderBy(d => d.DebtAmount).ToList()
                : query.OrderByDescending(d => d.DebtAmount).ToList();

            StateHasChanged();
        }

        // Method to clear all filters
        private void ClearFilters()
        {
            filterDescription = "";
            filterCategory = "";
            _selectedPaymentStatus = "";
            filterIsPaid = null;
            startDate = null;
            endDate = null;
            isSortedAscending = true;

            ApplyFilter(); //Reapply filters after clearing
        }

        // Method to show the modal for adding a new debt
        private void ShowAddDebtModal()
        {
            currentDebt = new ExpenseTracker.Models.Debt
            {
                WhenAdded = DateTime.Now,
                DueDate = DateTime.Now.AddMonths(1)
            };
            isEditMode = false;
            isModalVisible = true;
        }

        /// Method to show the modal for editing an existing debt
        private void ShowEditDebtModal(ExpenseTracker.Models.Debt debt)
        {
            currentDebt = new ExpenseTracker.Models.Debt
            {
                DebtId = debt.DebtId,
                DebtDescription = debt.DebtDescription,
                DebtAmount = debt.DebtAmount,
                DebtCategory = debt.DebtCategory,
                DueDate = debt.DueDate,
                IsPaid = debt.IsPaid,
                WhenAdded = debt.WhenAdded
            };
            isEditMode = true;
            isModalVisible = true;
        }

        // Method to close the modal
        private void CloseModal()
        {
            isModalVisible = false;
            currentDebt = new ExpenseTracker.Models.Debt();
            showBalanceError = false;
        }

        // Method to toggle the sort order of debts
        private void ToggleSort()
        {
            isSortedAscending = !isSortedAscending;
            ApplyFilter();
        }

        // Method to save the current debt (either adding or updating)
        private async Task SaveDebtAsync()
        {
            try
            {
                if (isEditMode)
                {
                    await DebtService.UpdateDebtAsync(currentDebt);
                }
                else
                {
                    await DebtService.AddDebtAsync(currentDebt);
                }

                await LoadDebtsAsync();
                ApplyFilter();
                CloseModal();
            }
            catch (Exception)
            {
                // Handle error appropriately
            }
        }

        // Method to mark a debt as paid, checking if the balance is sufficient
        private async Task MarkDebtAsPaidAsync(ExpenseTracker.Models.Debt debt)
        {
            if (debt.DebtAmount > currentBalance)
            {
                showBalanceError = true;
                balanceErrorMessage = "Insufficient balance to pay debt";
                return;
            }

            try
            {
                debt.IsPaid = true;
                currentBalance -= debt.DebtAmount; // Deduct from balance
                await DebtService.UpdateDebtAsync(debt);
                await LoadDebtsAsync();
                ApplyFilter();
            }
            catch (Exception)
            {
                // Handle error appropriately
            }
        }

        // Method to delete a debt
        private async Task DeleteDebtAsync(Guid debtId)
        {
            try
            {
                await DebtService.DeleteDebtAsync(debtId);
                await LoadDebtsAsync();
                ApplyFilter();
            }
            catch (Exception)
            {
                // Handle error appropriately
            }
        }

        // Method to export debts to a desired format  //Not working
        private void ExportDebts()
        {
            var debtsToExport = filteredDebts.Select(d => new
            {
                Description = d.DebtDescription,
                Amount = d.DebtAmount.ToString("C"),
                Category = d.DebtCategory,
                DueDate = d.DueDate.ToString("yyyy-MM-dd"),
                IsPaid = d.IsPaid ? "Paid" : "Pending",
                WhenAdded = d.WhenAdded.ToString("yyyy-MM-dd")
            }).ToList();
        }
    }
}