public class BalanceState
{
    public decimal CurrentBalance { get; private set; }
    public event Action OnBalanceChange;

    public void UpdateBalance(decimal newBalance)
    {
        CurrentBalance = newBalance;
        NotifyStateChanged();
    }

    private void NotifyStateChanged() => OnBalanceChange?.Invoke();
}