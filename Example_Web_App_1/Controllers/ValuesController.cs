using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Example_Web_App_1.Models;
using Microsoft.AspNetCore.Authorization;

namespace Example_Web_App_1.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        public EDBContext DBContext { get; }
        public ValuesController(EDBContext dBContext)
        {
            DBContext = dBContext;
        }
        // GET api/values
        [HttpGet]
        public IEnumerable<Account> Get()
        {
            return DBContext.Account;
            //return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
