namespace ExpenseTracker.Components.Pages
{
    public partial class Login
    {
        // Model to store login information like username and password
        private LoginModel loginModel = new LoginModel();

        // Default selected currency, initialized to "NPR"
        private string selectedCurrency = "NPR";

        // Variable to store error messages, empty by default
        private string errorMessage = string.Empty;

        // Method to handle login logic
        private void HandleLogin()
        {
            // Check if the provided username and password match hardcoded credentials
            if (loginModel.Username == "nerick" && loginModel.Password == "9847")
            {
                // Call the authentication service to log in the user
                AuthState.Login();

                // Navigate to the dashboard page after successful login
                Navigation.NavigateTo("/dashboard");
            }
            else
            {
                // Set an error message if the inputs are invalid
                errorMessage = "Username or password is wrong";
            }
        }

        // Class representing the login model
        public class LoginModel
        {
            // Property to store the username
            public string Username { get; set; }

            // Property to store the password
            public string Password { get; set; }
        }
    }
}
