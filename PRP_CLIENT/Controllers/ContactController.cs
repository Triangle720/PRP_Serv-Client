using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PRP_CLIENT.Management;
using PRP_CLIENT.Models;

namespace PRP_CLIENT.Controllers
{
    public class ContactController : Controller
    {
        public async Task<IActionResult> Index(long companyId, string searchName = "")
        {
            ViewBag.companyId = companyId;
            if(searchName == null || searchName == "")
            {
                List<Contact> contacts = await GetContactsAsync(companyId);
                return View(contacts);
            }
            else
            {
                List<Contact> contacts = await GetContactsAsync(companyId, searchName);
                return View(contacts);
            }
        }

        public async Task<Contact> GetContactAsync(int id)
        {
            Contact contact = new Contact();

            HttpResponseMessage responseMessage = await ConnectionManager.GetAsync("api/contacts/" + id.ToString(), HttpContext.Session.GetString("token"));

            if (responseMessage.IsSuccessStatusCode)
            {
                string userResponse = responseMessage.Content.ReadAsStringAsync().Result;
                contact = JsonConvert.DeserializeObject<Contact>(userResponse);
            }

            return contact;
        }

        public async Task<List<Contact>> GetContactsAsync(long companyId, string searchName = "")
        {
            List<Contact> contacts = new List<Contact>();
            HttpResponseMessage responseMessage;

            if(searchName == "")
                responseMessage = await ConnectionManager.GetAsync("api/contacts/company/" + companyId.ToString(), HttpContext.Session.GetString("token"));
            else
                responseMessage = await ConnectionManager.GetAsync("api/contacts/company/" + companyId.ToString() + "/name/" + searchName, HttpContext.Session.GetString("token"));

            if (responseMessage.IsSuccessStatusCode)
            {
                string userResponse = responseMessage.Content.ReadAsStringAsync().Result;
                contacts = JsonConvert.DeserializeObject<List<Contact>>(userResponse);
            }

            return contacts;
        }

        public ActionResult Create(long companyId)
        {
            ViewBag.companyId = companyId;
            return View();
        }

        public async Task<ActionResult> CreateContact(Contact contact, long companyId)
        {
            contact.User = new User();
            contact.User.Login = HttpContext.Session.GetString("login");

            try
            {
                HttpResponseMessage response = await ConnectionManager.PostAsync("api/contacts", contact);

                return RedirectToAction(nameof(Index), new { companyId = companyId });
            }
            catch
            {
                return View();
            }
        }

        public async Task<ActionResult> Edit(int id, long companyId)
        {
            ViewBag.companyId = companyId;
            Contact contact = await GetContactAsync(id);
            return View(contact);
        }

        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditContact(int id, Contact contact, long companyId)
        {
            contact.ContactId = id;
            if (HttpContext.Session.GetString("role") == "admin")
            {
                try
                {
                    HttpResponseMessage responseMessage = await ConnectionManager.PutAsync("api/contacts/" + id, contact, HttpContext.Session.GetString("token"));
                    return RedirectToAction(nameof(Index), new { companyId = companyId });
                }
                catch
                {
                    return View();
                }
            }
            else
            {
                try
                {
                    HttpResponseMessage responseMessage = await ConnectionManager.PutAsync("api/contacts/" + HttpContext.Session.GetString("login") + "/" + id, contact, HttpContext.Session.GetString("token"));
                    return RedirectToAction(nameof(Index), new { companyId = companyId });
                }
                catch
                {
                    return View();
                }
            }
        }

        public async Task<ActionResult> Delete(int id, long companyId)
        {
            ViewBag.companyId = companyId;
            Contact contact = await GetContactAsync(id);
            return View(contact);
        }

        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteContact(int id, long companyId)
        {
            if (HttpContext.Session.GetString("role") == "admin")
            {
                try
                {
                    HttpResponseMessage responseMessage = await ConnectionManager.DeleteAsync("api/contacts/" + id, HttpContext.Session.GetString("token"));
                    return RedirectToAction(nameof(Index), new { companyId = companyId });
                }
                catch
                {
                    return View();
                }
            }
            else
            {
                try
                {
                    HttpResponseMessage responseMessage = await ConnectionManager.DeleteAsync("api/contacts/" + HttpContext.Session.GetString("login") + "/" + id, HttpContext.Session.GetString("token"));
                    return RedirectToAction(nameof(Index), new { companyId = companyId });
                }
                catch
                {
                    return View();
                }
            }
        }

    }
}