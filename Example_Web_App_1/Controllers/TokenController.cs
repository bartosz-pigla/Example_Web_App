using Example_Web_App_1.Helpers;
using Example_Web_App_1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Example_Web_App_1.Controllers
{
    [Route("/token")]
    public class TokenController : Controller
    {
        public EDBContext DBContext { get; }
        public TokenController(EDBContext dBContext)
        {
            DBContext = dBContext;
        }

        [HttpPost]
        public IActionResult Create([FromBody] UserCredentials user)
        {
            if (IsValidUserAndPasswordCombination(user))
                return new ObjectResult(GenerateToken(user.Login));
            return BadRequest();
        }
        //[HttpPost]
        //public IActionResult Create(string username, string password)
        //{
        //    if (IsValidUserAndPasswordCombination(username, password))
        //        return new ObjectResult(GenerateToken(username));
        //    return BadRequest();
        //}
        private bool IsValidUserAndPasswordCombination(UserCredentials user)
        {
            var findUserQuery = from u in DBContext.Account where u.Login == user.Login select u.Password;
            string password = findUserQuery.SingleOrDefault();
            return password != null && password.Equals(user.Password);
        }

        //private bool IsValidUserAndPasswordCombination(string login, string password)
        //{

        //    return !string.IsNullOrEmpty(login) && login == password;
        //}

        private string GenerateToken(string username)
        {
            var claims = new Claim[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(JwtRegisteredClaimNames.Nbf, new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds().ToString()),
                new Claim(JwtRegisteredClaimNames.Exp, new DateTimeOffset(DateTime.Now.AddDays(1)).ToUnixTimeSeconds().ToString()),
            };

            var token = new JwtSecurityToken(
                new JwtHeader(new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes("the secret that needs to be at least 16 characeters long for HmacSha256")),
                                             SecurityAlgorithms.HmacSha256)),
                new JwtPayload(claims));

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
