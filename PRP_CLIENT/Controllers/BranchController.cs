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
    public class BranchController : Controller
    {
        public async Task<IActionResult> Index()
        {
            List<Branch> branches = await GetBranches();
            return View(branches);
        }

        public async Task<ActionResult> Create(int id)
        {
            Branch branch = await GetBranchAsync(id);
            return View(branch);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateBranch(Branch branch)
        {
            try
            {
                HttpResponseMessage response = await ConnectionManager.PostAsync("api/branches", branch);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        public async Task<List<Branch>> GetBranches()
        {
            List<Branch> branches = new List<Branch>();

            HttpResponseMessage responseMessage = await ConnectionManager.GetAsync("api/branches", HttpContext.Session.GetString("token"));

            if (responseMessage.IsSuccessStatusCode)
            {
                string userResponse = responseMessage.Content.ReadAsStringAsync().Result;
                branches = JsonConvert.DeserializeObject<List<Branch>>(userResponse);
            }

            return branches;
        }

        public async Task<ActionResult> Details(int id)
        {
            Branch branch = await GetBranchAsync(id);
            return View(branch);
        }

        public async Task<ActionResult> Edit(int id)
        {
            Branch branch = await GetBranchAsync(id);
            return View(branch);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditBranch(int id, Branch branch)
        {
            branch.BranchId = id;
            try
            {
                HttpResponseMessage responseMessage = await ConnectionManager.PutAsync("api/branches/" + id, branch, HttpContext.Session.GetString("token"));

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        public async Task<Branch> GetBranchAsync(int id)
        {
            Branch branch = new Branch();

            HttpResponseMessage responseMessage = await ConnectionManager.GetAsync("api/branches/" + id, HttpContext.Session.GetString("token"));

            if (responseMessage.IsSuccessStatusCode)
            {
                string userResponse = responseMessage.Content.ReadAsStringAsync().Result;
                branch = JsonConvert.DeserializeObject<Branch>(userResponse);
            }

            return branch;
        }

        public async Task<ActionResult> Delete(int id)
        {
            Branch branch = await GetBranchAsync(id);
            return View(branch);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteBranch(int id)
        {
            try
            {
                HttpResponseMessage responseMessage = await ConnectionManager.DeleteAsync("api/branches/" + id, HttpContext.Session.GetString("token"));
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}