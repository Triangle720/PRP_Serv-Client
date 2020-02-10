using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PRP_CLIENT.Models;
using PRP_CLIENT.Management;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;

namespace PRP_CLIENT.Controllers
{
    public class UserController : Controller
    {
        public async Task<IActionResult> Index(int id = 0)
        {
            ViewBag.page = id;
            List<User> users = await GetUsers(id);
            return View(users);
        }

        public async Task<List<User>> GetUsers(int p)
        {
            List<User> users = new List<User>();

            HttpResponseMessage responseMessage;

            responseMessage = await ConnectionManager.GetAsync("api/users/page/" + p.ToString(), HttpContext.Session.GetString("token"));

            if (responseMessage.IsSuccessStatusCode)
            {
                string userResponse = responseMessage.Content.ReadAsStringAsync().Result;
                users = JsonConvert.DeserializeObject<List<User>>(userResponse);
            }

            return users;
        }

        public async Task<ActionResult> Details(int id, int page)
        {
            ViewBag.page = page;
            User user = await GetUserAsync(id);
            return View(user);
        }

        public async Task<ActionResult> Edit(int id, int page)
        {
            ViewBag.page = page;
            User user = await GetUserAsync(id);
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditUser(int id, User user)
        {
            user.UserId = id;
            try
            {
                HttpResponseMessage responseMessage = await ConnectionManager.PutAsync("api/users/" + id, user, HttpContext.Session.GetString("token"));

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        public async Task<User> GetUserAsync(int id)
        {
            User user = new User();

            HttpResponseMessage responseMessage = await ConnectionManager.GetAsync("api/users/" + id, HttpContext.Session.GetString("token"));

            if (responseMessage.IsSuccessStatusCode)
            {
                string userResponse = responseMessage.Content.ReadAsStringAsync().Result;
                user = JsonConvert.DeserializeObject<User>(userResponse);
            }

            return user;
        }

        public async Task<ActionResult> Delete(int id, int page)
        {
            ViewBag.page = page;
            User user = await GetUserAsync(id);
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteUser(int id)
        {
            try
            {
                HttpResponseMessage responseMessage = await ConnectionManager.DeleteAsync("api/users/" + id, HttpContext.Session.GetString("token"));
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}