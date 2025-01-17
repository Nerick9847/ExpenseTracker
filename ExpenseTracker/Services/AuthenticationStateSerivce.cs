public class AuthenticationStateService
{
    private bool _isAuthenticated;
    public event Action? OnAuthenticationStateChanged;

    public bool IsAuthenticated
    {
        get => _isAuthenticated;
        private set
        {
            _isAuthenticated = value;
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