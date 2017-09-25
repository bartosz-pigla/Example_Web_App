using System;
using System.Collections.Generic;
using System.Web.Http;

namespace Example_Web_App_1.Models
{
    [Authorize]
    public partial class Account
    {
        public Account()
        {
            Note = new HashSet<Note>();
        }

        public int AccountId { get; set; }
        public string Login { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }

        public ICollection<Note> Note { get; set; }
    }
}
