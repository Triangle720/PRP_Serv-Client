using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PRP_CLIENT.Models;
using PRP_CLIENT.Management;
using System.Net.Http;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Net.Http.Headers;

namespace PRP_CLIENT.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {    
                return View();
        }

        public async Task<IActionResult> Login(User user)
        {
            string token = "";
            string apiUrl = "http://localhost:65268";
            HttpClient client = new HttpClient();

            client.BaseAddress = new Uri(apiUrl);
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = await client.PostAsJsonAsync("api/authorization/token", user);

            if (response.IsSuccessStatusCode)
            {
                token = response.Content.ReadAsStringAsync().Result;
            }

            if (token != "")
            {
                token = token.Remove(token.Length - 1);
                token = token.Substring(1);
                JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                JwtSecurityToken jwtToken = tokenHandler.ReadJwtToken(token);

                HttpContext.Session.SetString("login", jwtToken.Audiences.ToArray()[0]);
                HttpContext.Session.SetString("token", token);
                HttpContext.Session.SetString("role", jwtToken.Claims.First(x => x.Type.ToString().Equals(ClaimTypes.Role)).Value);
                HttpContext.Session.SetInt32("log", 1);


                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.LoginError = "Bad username or password";
                return View("Index");
            }

        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return View("Index");
        }
    }
}