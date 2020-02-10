using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PRP_SERVER.Models;
using PRP_SERVER.Management;

namespace PRP_SERVER.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorizationController : ControllerBase
    {
        private readonly DatabaseContext _context;

        public AuthorizationController(DatabaseContext context)
        {
            _context = context;
        }


        [HttpPost("token")]
        public ActionResult<string> GetToken(User body)
        {
            User user = new User(); ;

            try
            {
                user = _context.Users.Where(u => u.Login == body.Login).Single();
            }
            catch
            {
                return Unauthorized();
            }

            if(!MD5class.Check(body.Password, user.Password))
            {
                return Unauthorized();
            }

            string key = "agahkasdadluh!@asionm,cjvha!&^#a(wuhddj@nm,!#kjvlkl'l;la'v14125nljash";
            SymmetricSecurityKey symmetricKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            SigningCredentials signingCredentials = new SigningCredentials(symmetricKey, SecurityAlgorithms.HmacSha256Signature);

            List<Claim> claims = new List<Claim>();

            if (user.RoleId.Equals(1))
                claims.Add(new Claim(ClaimTypes.Role, "admin"));
            else if(user.RoleId.Equals(2))
                claims.Add(new Claim(ClaimTypes.Role, "user"));
            else
                claims.Add(new Claim(ClaimTypes.Role, "manager"));

            JwtSecurityToken token = new JwtSecurityToken(
                issuer: "INO",
                audience: user.Login.ToString(),
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: signingCredentials,
                claims: claims
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}