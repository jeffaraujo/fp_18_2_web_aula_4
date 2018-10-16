using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using fp_stack.api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace fp_stack.api.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        [HttpPost]
        public IActionResult Create([FromBody]LoginInfo model)
        {
            if (IsValidUserAndPasswordCombination(model.UserName, model.Password))
            {
                var token = GenerateToken(model.UserName);
                //Salvar no DB
                return new ObjectResult(token);

            }
            return BadRequest();
        }

        private bool IsValidUserAndPasswordCombination(string username, string password)
        {
            return username == "admin" && password == "senha";
        }


        private string GenerateToken(string username)
        {
            var claims = new Claim[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(JwtRegisteredClaimNames.Nbf, new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds().ToString()),
                new Claim(JwtRegisteredClaimNames.Exp, new DateTimeOffset(DateTime.Now.AddDays(1)).ToUnixTimeSeconds().ToString()),
            };

            var text = Encoding.UTF8.GetBytes("the secret that needs to be at least 16 characeters long for HmacSha256");
            var key = new SymmetricSecurityKey(text);
            var credential = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var header = new JwtHeader(credential);
            var payload = new JwtPayload(claims);
            var token = new JwtSecurityToken(header, payload);
            var handler = new JwtSecurityTokenHandler();


            return handler.WriteToken(token);


        }
    }
}