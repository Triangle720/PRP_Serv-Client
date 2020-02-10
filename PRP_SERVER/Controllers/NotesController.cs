using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PRP_SERVER.Models;

namespace PRP_SERVER.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotesController : ControllerBase
    {
        private readonly DatabaseContext _context;

        public NotesController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: api/Notes
        [Authorize(Roles = "admin,user,manager")]
        [HttpGet("company/{companyId}")]
        public async Task<ActionResult<IEnumerable<Note>>> GetNotes(long companyId)
        {
            return await _context.Notes.Where(n => n.CompanyId == companyId).ToListAsync();
        }

        // GET: api/Notes/5
        [Authorize(Roles = "admin,user,manager")]
        [HttpGet("{id}")]
        public async Task<ActionResult<Note>> GetNote(long id)
        {
            var note = await _context.Notes.FindAsync(id);

            if (note == null)
            {
                return NotFound();
            }

            return note;
        }

        [Authorize(Roles = "admin,manager")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutNote(long id, Note note)
        {
            if (id != note.NoteId)
            {
                return BadRequest();
            }

            _context.Entry(note).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NoteExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [Authorize(Roles = "user")]
        [HttpPut("{login}/{id}")]
        public async Task<IActionResult> PutNote(long id, Note note, string login)
        {
            if (id != note.NoteId)
            {
                return BadRequest();
            }

            if (_context.Notes.Where(c => c.NoteId == id && c.User.Name == login).Any())
            {
                _context.Entry(note).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NoteExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            return NoContent();
        }

        // POST: api/Notes
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [Authorize(Roles = "admin,user,manager")]
        [HttpPost]
        public async Task<ActionResult<Note>> PostNote(Note note)
        {
            note.IsDeleted = false;
            note.UserId = _context.Users.AsNoTracking().Where(u => u.Login == note.User.Login).Single().UserId;
            note.User = null;
            note.Company = null;

            _context.Notes.Add(note);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetNote", new { id = note.NoteId }, note);
        }

        // DELETE: api/Notes/5
        [Authorize(Roles = "admin,user,manager")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<Note>> DeleteNote(long id)
        {
            var note = await _context.Notes.FindAsync(id);
            if (note == null)
            {
                return NotFound();
            }

            _context.Notes.Remove(note);
            await _context.SaveChangesAsync();

            return note;
        }

        [Authorize(Roles = "user")]
        [HttpDelete("{login}/{id}")]
        public async Task<ActionResult<Note>> DeleteNote(long id, string login)
        {
            var note = await _context.Notes.FindAsync(id);

            if (note == null)
            {
                return NotFound();
            }

            if (_context.Notes.Where(c => c.NoteId == id && c.User.Name == login).Any())
            {
                _context.Notes.Remove(note);
                await _context.SaveChangesAsync();

                return note;
            }

            else
                return NotFound();
        }

        private bool NoteExists(long id)
        {
            return _context.Notes.Any(e => e.NoteId == id);
        }
    }
}
