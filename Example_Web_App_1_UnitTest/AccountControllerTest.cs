using Example_Web_App_1.Controllers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Example_Web_App_1.Helpers;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Net;
using Example_Web_App_1.Models;

namespace Example_Web_App_1_UnitTest
{
    public class AccountControllerTest
    {
        //private static List<Account> accounts = new List<Account>
        //{
        //        new Account { AccountId = 1, FirstName = "Alicja", LastName = "Abacka", Login = "alicja", Password = "alicja" },
        //        new Account { AccountId = 2, FirstName = "Krystyna", LastName = "Rzecka", Login = "krystyna", Password = "krystyna" },
        //        new Account { AccountId = 3, FirstName = "Wanda", LastName = "Grodzka", Login = "wanda", Password = "wanda" },
        //        new Account { AccountId = 4, FirstName = "Bogumila", LastName = "Jakas", Login = "bogumila", Password = "bogumila" },
        //        new Account { AccountId = 5, FirstName = "Anna", LastName = "Maracka", Login = "anna", Password = "anna" },
        //        new Account { AccountId = 6, FirstName = "Aleksandra", LastName = "Turowska", Login = "aleksandra", Password = "aleksandra" },
        //        new Account { AccountId = 7, FirstName = "Irena", LastName = "Szewinska", Login = "irena", Password = "irena" }
        //        //new Account { AccountId = 8, FirstName = "Eleonora", LastName = "Znana", Login = "eleonora", Password = "eleonora" }

        //};

        private static List<Account> accounts = new List<Account>
        {
                new Account { FirstName = "Alicja", LastName = "Abacka", Login = "alicja", Password = "alicja" },
                new Account { FirstName = "Krystyna", LastName = "Rzecka", Login = "krystyna", Password = "krystyna" },
                new Account { FirstName = "Wanda", LastName = "Grodzka", Login = "wanda", Password = "wanda" },
                new Account { FirstName = "Bogumila", LastName = "Jakas", Login = "bogumila", Password = "bogumila" },
                new Account { FirstName = "Anna", LastName = "Maracka", Login = "anna", Password = "anna" },
                new Account { FirstName = "Aleksandra", LastName = "Turowska", Login = "aleksandra", Password = "aleksandra" },
                new Account { FirstName = "Irena", LastName = "Szewinska", Login = "irena", Password = "irena" }
                //new Account { AccountId = 8, FirstName = "Eleonora", LastName = "Znana", Login = "eleonora", Password = "eleonora" }

        };
        public AccountControllerTest()
        {
            InitContext();
        }

        private EDBContext _edbContext;

        public void InitContext()
        {
            var builder = new DbContextOptionsBuilder<EDBContext>()
               .UseInMemoryDatabase(Guid.NewGuid().ToString())
               // don't raise the error warning us that the in memory db doesn't support transactions
               .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning));

            //var builder = new DbContextOptionsBuilder<EDBContext>()
            //    .UseInMemoryDatabase();

            var context = new EDBContext(builder.Options);

            var allAccounts = (from a in context.Account select a).ToList();
            if (allAccounts.Count != 0)
            {
                context.RemoveRange(allAccounts);
            }

            context.SaveChanges();
            context.Account.AddRange(accounts);
            int changed = context.SaveChanges();
            _edbContext = context;
        }

        [Fact]
        public void GetFirstUser()
        {
            string expectedLogin = "alicja";
            var controller = new AccountController(_edbContext);
            Account result = controller.Get(expectedLogin);
            Assert.Equal(result.Login, expectedLogin);
        }

        [Fact]
        public void GetSecondUser()
        {
            string expectedLogin = "krystyna";
            var controller = new AccountController(_edbContext);
            Account result = controller.Get(expectedLogin);
            Assert.Equal(result.Login, expectedLogin);
        }

        [Fact]
        public void GetFirstPageOfUsers()
        {
            List<Account> expectedUsers = new List<Account> { accounts[0], accounts[1] };
            var controller = new AccountController(_edbContext);
            List<Account> result = controller.Get(new Pagination(1, 2)).ToList();

            Assert.True(expectedUsers.SequenceEqual(result));
        }
        [Fact]
        public void GetLastPageOfUsers()
        {
            List<Account> expectedUsers = new List<Account> { accounts[6] };
            var controller = new AccountController(_edbContext);
            List<Account> result = controller.Get(new Pagination(4, 2)).ToList();

            Assert.True(expectedUsers.SequenceEqual(result));
        }
        [Fact]
        public void GetUnexistingPage()
        {
            List<Account> expectedUsers = new List<Account> { };
            var controller = new AccountController(_edbContext);
            List<Account> result = controller.Get(new Pagination(14, 2)).ToList();

            Assert.True(expectedUsers.SequenceEqual(result));
        }

        [Fact]
        public void PostUnexistingAccount()
        {
            Account account = new Account
            {
                FirstName = "Bartosz",
                LastName = "Pigla",
                Login = "bartosz",
                Password = "bartosz"
            };

            var controller = new AccountController(_edbContext);
            controller.Post(account);

            Assert.NotNull(_edbContext.Account.Select(a => a.Login == account.Login).FirstOrDefault());
        }

        [Fact]
        public void PostExistingAccount()
        {
            Account account = new Account { FirstName = "Alicja", LastName = "Abacka", Login = "alicja", Password = "alicja" };
            var controller = new AccountController(_edbContext);

            var exception = Assert.Throws<System.Web.Http.HttpResponseException>(() => controller.Post(account));
            Assert.Equal(exception.Response.StatusCode, HttpStatusCode.Conflict);
        }

        [Fact]
        public void PutExistingAccount()
        {
            Account account = new Account { FirstName = "Krystyna", LastName = "¯ecka", Login = "krystyna", Password = "krystyna" };

            var controller = new AccountController(_edbContext);
            controller.Put(account);

            Assert.True((from a in _edbContext.Account where a.Login == "krystyna" select a).FirstOrDefault().LastName.Equals("¯ecka"));
        }

        [Fact]
        public void PutUnexistingAccount()
        {
            Account account = new Account
            {
                FirstName = "Bartosz",
                LastName = "Pigla",
                Login = "bartosz",
                Password = "bartosz"
            };

            var controller = new AccountController(_edbContext);

            var exception = Assert.Throws<System.Web.Http.HttpResponseException>(() => controller.Put(account));
            Assert.Equal(exception.Response.StatusCode, HttpStatusCode.NotFound);
        }

        [Fact]
        public void DeleteExistingAccount()
        {
            Account account = accounts[1];

            var controller = new AccountController(_edbContext);
            controller.Delete(account.Login);

            Account result = (from a in _edbContext.Account where a.Login == "krystyna" select a).FirstOrDefault();

            Assert.Null(result);
        }

        [Fact]
        public void DeleteUnexistingAccount()
        {
            Account account = new Account
            {
                FirstName = "Bartosz",
                LastName = "Pigla",
                Login = "bartosz",
                Password = "bartosz"
            };

            var controller = new AccountController(_edbContext);

            var exception = Assert.Throws<System.Web.Http.HttpResponseException>(() => controller.Delete(account.Login));
            Assert.Equal(exception.Response.StatusCode, HttpStatusCode.NotFound);
        }
    }
}
