using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Example_Web_App_1.Helpers;
using System.Net;
using Example_Web_App_1.Models;
using Microsoft.AspNetCore.Authorization;

namespace Example_Web_App_1.Controllers
{

    [Route("api/[controller]")]
    public class AccountController : Controller
    {
        public EDBContext DBContext { get; }
        public AccountController(EDBContext dBContext)
        {
            DBContext = dBContext;
        }

        //public AccountController()
        //{

        //}
        [Authorize]
        [HttpGet]
        public IEnumerable<Account> Get([System.Web.Http.FromUri] Pagination pagination)
        {
            return DBContext.Account.Skip((pagination.Number - 1) * pagination.Size).Take(pagination.Size);
        }
        [Authorize]
        [HttpGet("{login}")]
        public Account Get(string login)
        {
            var query =
                from p in DBContext.Account
                where p.Login == login
                select p;
            return query.FirstOrDefault();
        }
        
        [HttpPost]
        public void Post([FromBody]Account newAccount)
        {
            using (var transaction = DBContext.Database.BeginTransaction())
            {
                var checkIfAccountAlreadyExistsQuery =
                    from p in DBContext.Account
                    where p.Login == newAccount.Login
                    select p;

                if (checkIfAccountAlreadyExistsQuery.FirstOrDefault() == null)
                {
                    DBContext.Account.Add(newAccount);
                    DBContext.SaveChanges();
                    transaction.Commit();
                }
                else
                {
                    transaction.Rollback();
                    throw new System.Web.Http.HttpResponseException(HttpStatusCode.Conflict);
                }
            }
        }
        [Authorize]
        [HttpPut]
        public void Put([FromBody]Account updatedAccount)
        {
            using (var transaction = DBContext.Database.BeginTransaction())
            {
                var checkIfAccountAlreadyExistsQuery =
                    from p in DBContext.Account
                    where p.AccountId == updatedAccount.AccountId
                    select p;

                Account accountToUpdate = checkIfAccountAlreadyExistsQuery.FirstOrDefault();

                if (accountToUpdate != null)
                {
                    accountToUpdate.FirstName = updatedAccount.FirstName;
                    accountToUpdate.LastName = updatedAccount.LastName;
                    accountToUpdate.Password = updatedAccount.Password;
                    accountToUpdate.Login = updatedAccount.Login;

                    DBContext.Account.Update(accountToUpdate);
                    DBContext.SaveChanges();
                    transaction.Commit();
                }
                else
                {
                    transaction.Rollback();
                    throw new System.Web.Http.HttpResponseException(HttpStatusCode.NotFound);
                }
            }
        }
        [Authorize]
        [HttpDelete("{login}")]
        public void Delete(string login)
        {
            using (var transaction = DBContext.Database.BeginTransaction())
            {
                var checkIfAccountAlreadyExistsQuery =
                    from p in DBContext.Account
                    where p.Login == login
                    select p;

                Account accountToDelete = checkIfAccountAlreadyExistsQuery.FirstOrDefault();

                if (accountToDelete != null)
                {
                    DBContext.Account.Remove(accountToDelete);
                    DBContext.SaveChanges();
                    transaction.Commit();
                }
                else
                {
                    transaction.Rollback();
                    throw new System.Web.Http.HttpResponseException(HttpStatusCode.NotFound);
                }
            }
        }
    }
}
