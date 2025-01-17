public class AuthenticationStateService
{
    private bool _isAuthenticated; // Tracks the current authentication state

    public event Action? OnAuthenticationStateChanged;

    public bool IsAuthenticated
    {
        get => _isAuthenticated;
        private set
        {
            _isAuthenticated = value;
            // Notify authentication state changes
            OnAuthenticationStateChanged?.Invoke();
        }
    }

    public void Login()
    {
        IsAuthenticated = true;
    }

    public void Logout()
    {
        IsAuthenticated = false;
    }
}
