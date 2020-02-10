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
    public class NoteController : Controller
    {
        public async Task<IActionResult> Index(long companyId)
        {
            ViewBag.companyId = companyId;
            List<Note> notes = await GetNotesAsync(companyId);
            return View(notes);
        }

        public async Task<Note> GetNoteAsync(int id)
        {
            Note note = new Note();

            HttpResponseMessage responseMessage = await ConnectionManager.GetAsync("api/notes/" + id.ToString(), HttpContext.Session.GetString("token"));

            if (responseMessage.IsSuccessStatusCode)
            {
                string userResponse = responseMessage.Content.ReadAsStringAsync().Result;
                note = JsonConvert.DeserializeObject<Note>(userResponse);
            }

            return note;
        }

        public async Task<List<Note>> GetNotesAsync(long companyId)
        {
            List<Note> notes = new List<Note>();
            HttpResponseMessage responseMessage;

            responseMessage = await ConnectionManager.GetAsync("api/notes/company/" + companyId.ToString(), HttpContext.Session.GetString("token"));

            if (responseMessage.IsSuccessStatusCode)
            {
                string userResponse = responseMessage.Content.ReadAsStringAsync().Result;
                notes = JsonConvert.DeserializeObject<List<Note>>(userResponse);
            }

            return notes;
        }

        public ActionResult Create(long companyId)
        {
            ViewBag.companyId = companyId;
            return View();
        }

        public async Task<ActionResult> CreateNote(Note note, long companyId)
        {
            note.User = new User();
            note.User.Login = HttpContext.Session.GetString("login");

            try
            {
                HttpResponseMessage response = await ConnectionManager.PostAsync("api/notes", note);

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
            Note note = await GetNoteAsync(id);
            return View(note);
        }

        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditNote(int id, Note note, long companyId)
        {
            note.NoteId = id;
            if (HttpContext.Session.GetString("role") == "admin")
            {
                try
                {
                    HttpResponseMessage responseMessage = await ConnectionManager.PutAsync("api/notes/" + id, note, HttpContext.Session.GetString("token"));
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
                    HttpResponseMessage responseMessage = await ConnectionManager.PutAsync("api/notes/" + HttpContext.Session.GetString("login") + "/" + id, note, HttpContext.Session.GetString("token"));
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
            Note note = await GetNoteAsync(id);
            return View(note);
        }

        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteNote(int id, long companyId)
        {
            if (HttpContext.Session.GetString("role") == "admin")
            {
                try
                {
                    HttpResponseMessage responseMessage = await ConnectionManager.DeleteAsync("api/notes/" + id, HttpContext.Session.GetString("token"));
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
                    HttpResponseMessage responseMessage = await ConnectionManager.DeleteAsync("api/notes/" + HttpContext.Session.GetString("login") + "/" + id, HttpContext.Session.GetString("token"));
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