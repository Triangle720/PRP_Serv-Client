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
    public class CompanyController : Controller
    {
        // GET: Company
        public async Task<ActionResult> Index(int id = 0, string searchBranch = "")
        {
            ViewBag.page = id;
            if (searchBranch == null || searchBranch == "")
            {
                List<Company> companies = await GetCompaniesAsync(id);
                return View(companies);
            }
            else
            {
                List<Company> companies = await GetCompaniesAsync(id, searchBranch);
                return View(companies);
            }
        }

        public async Task<Company> GetCompanyAsync(int id)
        {
            Company company = new Company();

            HttpResponseMessage responseMessage = await ConnectionManager.GetAsync("api/companies/" + id.ToString(), HttpContext.Session.GetString("token"));

            if (responseMessage.IsSuccessStatusCode)
            {
                string userResponse = responseMessage.Content.ReadAsStringAsync().Result;
                company = JsonConvert.DeserializeObject<Company>(userResponse);
            }

            return company;
        }

        public async Task<List<Company>> GetCompaniesAsync(int p, string branch = "")
        {
            List<Company> companies = new List<Company>();
            HttpResponseMessage responseMessage;

            if(branch == "")
                responseMessage = await ConnectionManager.GetAsync("api/companies/page/" + p.ToString(), HttpContext.Session.GetString("token"));
            else
                responseMessage = await ConnectionManager.GetAsync("api/companies/branch/" + branch + "/page/" + p.ToString(), HttpContext.Session.GetString("token"));

            if (responseMessage.IsSuccessStatusCode)
            {
                string userResponse = responseMessage.Content.ReadAsStringAsync().Result;
                companies = JsonConvert.DeserializeObject<List<Company>>(userResponse);
            }

            return companies;
        }

        public async Task<ActionResult> Details(int id, int page)
        {
            ViewBag.page = page;
            Company company = await GetCompanyAsync(id);
            return View(company);
        }

        // GET: Company/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Company/Create
        public async Task<ActionResult> CreateCompany(Company company)
        {
            company.User = new User();
            company.User.Login = HttpContext.Session.GetString("login");

            try
            {
                HttpResponseMessage response = await ConnectionManager.PostAsync("api/companies", company);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Company/Edit/5
        public async Task<ActionResult> Edit(int id, int page)
        {
            ViewBag.page = page;
            Company company = await GetCompanyAsync(id);
            return View(company);
        }

        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditCompany(int id, Company company)
        {
            company.CompanyId = id;
            if (HttpContext.Session.GetString("role") == "admin")
            {
                try
                {
                    HttpResponseMessage responseMessage = await ConnectionManager.PutAsync("api/companies/" + id, company, HttpContext.Session.GetString("token"));
                    return RedirectToAction(nameof(Index));
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
                    HttpResponseMessage responseMessage = await ConnectionManager.PutAsync("api/companies/" + HttpContext.Session.GetString("login") + "/" + id, company, HttpContext.Session.GetString("token"));
                    return RedirectToAction(nameof(Index));
                }
                catch
                {
                    return View();
                }
            }
        }

        public async Task<ActionResult> Delete(int id, int page)
        {
            ViewBag.page = page;
            Company company = await GetCompanyAsync(id);
            return View(company);
        }

        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteCompany(int id)
        {
            if (HttpContext.Session.GetString("role") == "admin")
            {
                try
                {
                    HttpResponseMessage responseMessage = await ConnectionManager.DeleteAsync("api/companies/" + id, HttpContext.Session.GetString("token"));
                    return RedirectToAction(nameof(Index));
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
                    HttpResponseMessage responseMessage = await ConnectionManager.DeleteAsync("api/companies/" + HttpContext.Session.GetString("login") + "/" + id, HttpContext.Session.GetString("token"));
                    return RedirectToAction(nameof(Index));
                }
                catch
                {
                    return View();
                }
            }
        }
    }
}