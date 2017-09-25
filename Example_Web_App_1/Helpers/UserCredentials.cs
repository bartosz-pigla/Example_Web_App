using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Example_Web_App_1.Helpers
{
    public class UserCredentials
    {
        public string Login { get; set; }
        public string Password { get; set; }

        public UserCredentials(string username, string password)
        {
            Login = username;
            Password = password;
        }

        public UserCredentials()
        {
        }
    }
}
