public class BalanceState
{
    // Holds the current balance
    public decimal CurrentBalance { get; private set; }

    // Event triggered when the balance changes
    public event Action OnBalanceChange;

    // Method to update the balance and notify listeners
    public void UpdateBalance(decimal newBalance)
    {
        CurrentBalance = newBalance;
        NotifyStateChanged(); // Notify listeners about the balance change
    }

    // Invokes the OnBalanceChange event to notify listeners
    private void NotifyStateChanged() => OnBalanceChange?.Invoke();
}
