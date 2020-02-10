using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PRP_CLIENT.Management;
using PRP_CLIENT.Models;

namespace PRP_CLIENT.Controllers
{
    public class RegisterController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Register(User newUser)
        {
            if (newUser.Login.Equals("") || newUser.Password.Equals(""))
            {
                ViewBag.LoginError = "Nie można zarejestrować użytkownika o takich parametrach";
                return View("Index");
            }

            newUser.IsDeleted = false;
            newUser.RoleId = 2;

            newUser.Password = MD5class.Create(newUser.Password);

            HttpResponseMessage Res = await ConnectionManager.RegisterUser(newUser);

            if (Res.IsSuccessStatusCode)
            {
                ViewBag.LoginError = "Pomyślnie zarejestrowano!";
                return View("Index");
            }

            ViewBag.LoginError = "Nie można zarejestrować użytkownika o takich parametrach";
            return View("Index");
        }
    }
}