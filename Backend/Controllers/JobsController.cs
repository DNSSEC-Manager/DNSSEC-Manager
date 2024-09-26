using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Backend.Data;
using Backend.Models;

namespace Backend.Controllers
{
    public class JobsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public JobsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Jobs
        public async Task<IActionResult> Index(string id, string sort)
        {
            var filter = id;
            var nonPerms = _context.Jobs.Where(b => !b.IsPermanent);
            switch (filter)
            {
                case "all": break;
                case "finished":
                    nonPerms = nonPerms.Where(b => b.IsCompleted);
                    break;
                case "unsuccessfull":
                    nonPerms = nonPerms.Where(b => !b.IsSuccessful && b.UpdatedAt != DateTime.MinValue);
                    break;
                default:
                    nonPerms = nonPerms.Where(b => !b.IsCompleted);
                    filter = "unfinished";
                    break;
            }

            switch (sort)
            {
                case "task":
                    nonPerms = nonPerms.OrderBy(b => b.IsSuccessful).ThenBy(b => b.Task);
                    break;
                case "task_desc":
                    nonPerms = nonPerms.OrderBy(b => b.IsSuccessful).ThenByDescending(b => b.Task);
                    break;
                case "successful":
                    nonPerms = nonPerms.OrderBy(b => b.IsSuccessful);
                    break;
                case "successful_desc":
                    nonPerms = nonPerms.OrderByDescending(b => b.IsSuccessful);
                    break;
                case "creationDate":
                    nonPerms = nonPerms.OrderBy(b => b.IsSuccessful).ThenBy(b => b.CreatedAt);
                    break;
                case "creationDate_desc":
                    nonPerms = nonPerms.OrderBy(b => b.IsSuccessful).ThenByDescending(b => b.CreatedAt);
                    break;
                case "lastExecuted":
                    nonPerms = nonPerms.OrderBy(b => b.IsSuccessful).ThenBy(b => b.UpdatedAt);
                    break;
                case "lastExecuted_desc":
                    nonPerms = nonPerms.OrderBy(b => b.IsSuccessful).ThenByDescending(b => b.UpdatedAt);
                    break;
                case "nextdate_desc":
                    nonPerms = nonPerms.OrderBy(b => b.IsSuccessful).ThenByDescending(b => b.RunAfter);
                    break;
                default:
                    nonPerms = nonPerms.OrderBy(b => b.IsSuccessful).ThenBy(b => b.RunAfter);
                    break;
            }
            ViewData["permanentJobs"] = await _context.Jobs.Include(b => b.DnsServer).Where(b => b.IsPermanent).ToListAsync();
            ViewData["nonPermanentJobs"] = await nonPerms.Include(b => b.Domain).ToListAsync();
            ViewData["sort"] = sort;
            ViewData["filter"] = filter;
            return View();
        }


        // GET: Jobs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var job = await _context.Jobs
                .Include(j => j.DnsServer)
                .Include(j => j.Domain)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (job == null)
            {
                return NotFound();
            }

            return View(job);
        }

        // GET: Jobs/Create
        public IActionResult Create()
        {
            ViewData["DnsServerId"] = new SelectList(_context.DnsServers, "Id", "Name");
            ViewData["DomainId"] = new SelectList(_context.Domains, "Id", "Name");
            return View();
        }

        // POST: Jobs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,DnsServerId,DomainId,Task,IsPermanent,IsSuccessful,CreatedAt,RunAfter,UpdatedAt")] Job job)
        {
            if (ModelState.IsValid)
            {
                _context.Add(job);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DnsServerId"] = new SelectList(_context.DnsServers, "Id", "Name", job.DnsServerId);
            ViewData["DomainId"] = new SelectList(_context.Domains, "Id", "Name", job.DomainId);
            return View(job);
        }

        // GET: Jobs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var job = await _context.Jobs.FindAsync(id);
            if (job == null)
            {
                return NotFound();
            }
            ViewData["DnsServerId"] = new SelectList(_context.DnsServers, "Id", "Name", job.DnsServerId);
            ViewData["DomainId"] = new SelectList(_context.Domains, "Id", "Name", job.DomainId);
            return View(job);
        }

        // POST: Jobs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, bool IsCompleted, DateTime RunAfter)
        {
            Job job;
            try
            {
                job = _context.Jobs.Find(id);
            }
            catch (Exception)
            {
                return NotFound();
            }
            
            if (ModelState.IsValid)
            {
                try
                {
                    job.IsCompleted = IsCompleted;
                    job.RunAfter = RunAfter;
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!JobExists(id))
                    {
                        return NotFound();
                    }
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }

            //ViewData["DnsServerId"] = new SelectList(_context.DnsServers, "Id", "Name", job.DnsServerId);
            //ViewData["DomainId"] = new SelectList(_context.Domains, "Id", "Name", job.DomainId);
            return View(job);
        }

        // GET: Jobs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var job = await _context.Jobs
                .Include(j => j.DnsServer)
                .Include(j => j.Domain)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (job == null)
            {
                return NotFound();
            }

            return View(job);
        }

        // POST: Jobs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var job = await _context.Jobs.FindAsync(id);
            _context.Jobs.Remove(job);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool JobExists(int id)
        {
            return _context.Jobs.Any(e => e.Id == id);
        }
    }
}
