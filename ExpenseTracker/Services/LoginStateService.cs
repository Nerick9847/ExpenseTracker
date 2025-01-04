using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Services
{
    public class LoginState
    {
        public bool IsLoggedIn { get; private set; } = false;

        public void Login()
        {
            IsLoggedIn = true;
        }

        public void Logout()
        {
            IsLoggedIn = false;
        }
    }

}
