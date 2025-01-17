namespace ExpenseTracker.Components.Pages
{
    public partial class Login
    {
        private LoginModel loginModel = new LoginModel();
        private string selectedCurrency = "NPR";
        private string errorMessage = string.Empty;

        private void HandleLogin()
        {
            if (loginModel.Username == "nerick" && loginModel.Password == "9847")
            {
                AuthState.Login();
                Navigation.NavigateTo("/dashboard");
            }
            else
            {
                errorMessage = "Username or password is wrong";
            }
        }

        public class LoginModel
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }
    }
}