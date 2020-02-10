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
    public class CompaniesController : ControllerBase
    {
        private readonly DatabaseContext _context;

        public CompaniesController(DatabaseContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "admin,user")]
        [Route("page/{id}")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Company>>> GetCompanies(int id)
        {
            int from = id * 10;
            List<Company> companies = await _context.Companies.ToListAsync();
            int lenght = companies.Count;

            if (lenght >= from + 10)
                return companies.GetRange(from, 10);
            else
                return companies.GetRange(from, lenght - from);
        }

        [Authorize(Roles = "admin,user")]
        [Route("branch/{branch}/page/{id}")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Company>>> GetCompanies(string branch, int id)
        {
            List<Company> companies =  await _context.Companies.Where(c => c.Branch.Name == branch).ToListAsync();
            int from = id * 10;
            int lenght = companies.Count;

            if (lenght >= from + 10)
                return companies.GetRange(from, 10);
            else
                return companies.GetRange(from, lenght - from);
        }

        [Authorize(Roles = "admin,user")]
        [Route("{id}")]
        [HttpGet]
        public async Task<ActionResult<Company>> GetCompany(long id)
        {
            var company = await _context.Companies.FindAsync(id);

            if (company == null)
            {
                return NotFound();
            }

            return company;
        }

        [Authorize(Roles = "admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCompany(long id, Company company)
        {
            if (id != company.CompanyId)
            {
                return BadRequest();
            }

            if (!BranchExists(company.Branch.Name))
            {
                _context.Branches.Add(company.Branch);
                await _context.SaveChangesAsync();
            }

            company.BranchId = _context.Branches.Where(b => b.Name == company.Branch.Name).Single().BranchId;
            company.Branch = null;
            company.UserId = _context.Companies.AsNoTracking().Where(c => c.CompanyId == company.CompanyId).Single().UserId;

            _context.Entry(company).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CompanyExists(id))
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
        public async Task<IActionResult> PutCompany(long id, Company company, string login)
        {
            if (id != company.CompanyId)
            {
                return BadRequest();
            }

            if (!BranchExists(company.Branch.Name))
            {
                _context.Branches.Add(company.Branch);
                await _context.SaveChangesAsync();
            }

            if(_context.Companies.Where(c => c.CompanyId == id && c.User.Name == login).Any())
            {
                company.BranchId = _context.Branches.Where(b => b.Name == company.Branch.Name).Single().BranchId;
                company.Branch = null;
                company.UserId = _context.Companies.AsNoTracking().Where(c => c.CompanyId == company.CompanyId).Single().UserId;

                _context.Entry(company).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CompanyExists(id))
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

        [Authorize(Roles = "admin,user")]
        [HttpPost]
        public async Task<ActionResult<Company>> PostCompany(Company company)
        {
            company.IsDeleted = false;

            if (!BranchExists(company.Branch.Name))
            {
                await _context.SaveChangesAsync();
            }

            company.BranchId = _context.Branches.Where(b => b.Name == company.Branch.Name).Single().BranchId;
            company.UserId = _context.Users.Where(u => u.Login == company.User.Login).Single().UserId;
            company.User = null;
            company.Branch = null;
            _context.Companies.Add(company);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCompanyModel", new { id = company.CompanyId }, company);
        }

        private bool BranchExists(string name)
        {
            return _context.Branches.Any(b => b.Name == name);
        }

        // DELETE: api/Companies/5
        [Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<Company>> DeleteCompany(long id)
        {
            var company = await _context.Companies.FindAsync(id);
            if (company == null)
            {
                return NotFound();
            }

            _context.Companies.Remove(company);
            await _context.SaveChangesAsync();

            return company;
        }

        [Authorize(Roles = "user")]
        [HttpDelete("{login}/{id}")]
        public async Task<ActionResult<Company>> DeleteCompany(long id, string login)
        {
            var company = await _context.Companies.FindAsync(id);

            if (company == null)
            {
                return NotFound();
            }

            if (_context.Companies.Where(c => c.CompanyId == id && c.User.Name == login).Any())
            {
                _context.Companies.Remove(company);
                await _context.SaveChangesAsync();

                return company;
            }

            else
                return NotFound();
        }

        private bool CompanyExists(long id)
        {
            return _context.Companies.Any(e => e.CompanyId == id);
        }
    }
}
