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
    public class ContactsController : ControllerBase
    {
        private readonly DatabaseContext _context;

        public ContactsController(DatabaseContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "admin,user,manager")]
        [HttpGet("company/{companyId}")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Contact>>> GetContacts(int companyId)
        {
            return await _context.Contacts.Where(c => c.CompanyId == companyId).ToListAsync();
        }

        [Authorize(Roles = "admin,user,manager")]
        [HttpGet("company/{companyId}/name/{name}")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Contact>>> GetContacts(int companyId, string name)
        {
            List<Contact> contacts = await _context.Contacts.Where(c => c.CompanyId == companyId && c.Surname == name).ToListAsync();
            return contacts;
        }

        [Authorize(Roles = "admin,user,manager")]
        [HttpGet("{id}")]
        public async Task<ActionResult<Contact>> GetContact(long id)
        {
            var contact = await _context.Contacts.FindAsync(id);

            if (contact == null)
            {
                return NotFound();
            }

            return contact;
        }

        // PUT: api/Contacts/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [Authorize(Roles = "admin,manager,user")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutContact(long id, Contact contact)
        {
            if (id != contact.ContactId)
            {
                return BadRequest();
            }

            _context.Entry(contact).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ContactExists(id))
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
        public async Task<IActionResult> PutContact(long id, Contact contact, string login)
        {
            if (id != contact.ContactId)
            {
                return BadRequest();
            }

            if(_context.Contacts.Where(c => c.User.Name == login && c.ContactId == id).Any())
            {
                _context.Entry(contact).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContactExists(id))
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

            return NoContent();
        }

        [Authorize(Roles = "admin,user,manager")]
        [HttpPost]
        public async Task<ActionResult<Contact>> PostContact(Contact contact)
        {
            contact.IsDeleted = false;
            contact.UserId = _context.Users.AsNoTracking().Where(u => u.Login == contact.User.Login).Single().UserId;
            contact.User = null;
            contact.Company = null;
            _context.Contacts.Add(contact);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetContact", new { id = contact.ContactId }, contact);
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<Contact>> DeleteContact(long id)
        {
            var contact = await _context.Contacts.FindAsync(id);
            if (contact == null)
            {
                return NotFound();
            }

            _context.Contacts.Remove(contact);
            await _context.SaveChangesAsync();

            return contact;
        }

        [Authorize(Roles = "user")]
        [HttpDelete("{login}/{id}")]
        public async Task<ActionResult<Contact>> DeleteContact(long id, string login)
        {
            var contact = await _context.Contacts.FindAsync(id);

            if(_context.Contacts.Where(c => c.User.Name == login && c.ContactId == id).Any())
            {
                if (contact == null)
                {
                    return NotFound();
                }

                _context.Contacts.Remove(contact);
                await _context.SaveChangesAsync();
            }

            return contact;
        }

        private bool ContactExists(long id)
        {
            return _context.Contacts.Any(e => e.ContactId == id);
        }
    }
}
