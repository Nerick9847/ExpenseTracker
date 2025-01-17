using ExpenseTracker.Services;
using MudBlazor;

namespace ExpenseTracker.Components.Pages
{
    public partial class Debt
    {
        private List<ExpenseTracker.Models.Debt> debts = new();
        private List<ExpenseTracker.Models.Debt> filteredDebts = new();
        private ExpenseTracker.Models.Debt currentDebt = new();
        private bool showBalanceError = false;
        private string balanceErrorMessage = string.Empty;
        private bool isModalVisible = false;
        private bool isEditMode = false;
        private string filterDescription = "";
        private string filterCategory = "";
        private string _selectedPaymentStatus = "";
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
        private bool? filterIsPaid = null;
        private DateTime? startDate = null;
        private DateTime? endDate = null;
        private List<string> categories = new List<string> { "Personal", "Business", "Student Loan", "Home Loan" };
        private bool isSortedAscending = true;
        private decimal currentBalance = 1000m; // Example balance, replace with your actual balance retrieval logic

        protected override async Task OnInitializedAsync()
        {
            await LoadDebtsAsync();
            ApplyFilter();
        }

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

        private void ApplyFilter()
        {
            var query = debts.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filterDescription))
            {
                query = query.Where(d => d.DebtDescription != null &&
                    d.DebtDescription.Contains(filterDescription, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(filterCategory))
            {
                query = query.Where(d => d.DebtCategory != null &&
                    d.DebtCategory.Equals(filterCategory, StringComparison.OrdinalIgnoreCase));
            }

            if (filterIsPaid.HasValue)
            {
                query = query.Where(d => d.IsPaid == filterIsPaid.Value);
            }

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

            filteredDebts = isSortedAscending
                ? query.OrderBy(d => d.DebtAmount).ToList()
                : query.OrderByDescending(d => d.DebtAmount).ToList();

            StateHasChanged();
        }

        private void ClearFilters()
        {
            filterDescription = "";
            filterCategory = "";
            _selectedPaymentStatus = "";
            filterIsPaid = null;
            startDate = null;
            endDate = null;
            isSortedAscending = true;

            ApplyFilter();
        }

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

        private void CloseModal()
        {
            isModalVisible = false;
            currentDebt = new ExpenseTracker.Models.Debt();
            showBalanceError = false;
        }

        private void ToggleSort()
        {
            isSortedAscending = !isSortedAscending;
            ApplyFilter();
        }

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

        private async Task MarkDebtAsPaidAsync(ExpenseTracker.Models.Debt debt)
        {
            if (debt.DebtAmount > currentBalance)
            {
                showBalanceError = true;
                balanceErrorMessage = "Insufficient balance to mark this debt as paid.";
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

            // Implement export logic here
        }
    }
}