using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Backend.Data;
using Backend.Models;
using Backend.ViewModels;
using Microsoft.EntityFrameworkCore.Query;

namespace Backend.Controllers
{
    public class LogsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LogsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Logs
        public async Task<IActionResult> Index(int? pageNumber, int? logType, string sort, int? dnsServerId, int? pageSize)
        {
            IQueryable<Log> logs = _context.Logs
                .Include(l => l.DnsServer)
                .Include(l => l.Domain)
                .Include(l => l.Job)
                .Include(l => l.Registry);

            var actualLogType = logType ?? 0;

            if (actualLogType != 0)
            {
                logs =  logs.Where(b => b.LogType == (LogType) actualLogType);
            }

            switch (sort)
            {
                case "CreatedAt":
                    logs = logs.OrderBy(d => d.CreatedAt);
                    break;
                case "Message":
                    logs = logs.OrderBy(d => d.Message);
                    break;
                case "Message_desc":
                    logs = logs.OrderByDescending(d => d.Message);
                    break;
                case "Domain":
                    logs = logs.OrderBy(d => d.Domain.Name);
                    break;
                case "Domain_desc":
                    logs = logs.OrderByDescending(d => d.Domain.Name);
                    break;
                default:
                    sort = "CreatedAt_desc";
                    logs = logs.OrderByDescending(d => d.CreatedAt);
                    break;

            }

            switch (dnsServerId)
            {
                case -1:
                    logs = logs.Where(b => b.DnsServerId == null);
                    break;
                case 0:
                    logs = logs.Where(b => b.DnsServerId != null);
                    break;
                default:
                    if (dnsServerId != null)
                    {
                        logs = logs.Where(b => b.DnsServerId == dnsServerId);
                    }
                    break;
            }

            int actualPageSize;

            if (pageSize == null)
            {
               actualPageSize = 10;
            }
            else
            {
                actualPageSize = (int) pageSize;
            }

            var page = pageNumber ?? 1;
            var skipNum = (page - 1) * actualPageSize;
            var totalLogs = logs.Count();
            var logsList = await logs.Skip(skipNum).Take(actualPageSize).ToListAsync();
            var totalPages = (int)Math.Ceiling(totalLogs / (double)actualPageSize);

            var hasNextPage = false;
            var hasPreviousPage = pageNumber > 1;

            if (page < totalPages)
            {
                hasNextPage = true;
            }

            var dnsServers = await _context.DnsServers.ToListAsync();
            var vm = new LogsViewModel
            {
                Logs = logsList,
                PageNumber = page,
                HasNextPage = hasNextPage,
                HasPreviousPage = hasPreviousPage,
                TotalPages = totalPages,
                Sort = sort,
                LogType = actualLogType,
                DnsServerId = dnsServerId,
                DnsServers = dnsServers,
                PageSize = actualPageSize
            };

            return View(vm);
        }

        // GET: Logs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var log = await _context.Logs
                .Include(l => l.DnsServer)
                .Include(l => l.Domain)
                .Include(l => l.Job)
                .Include(l => l.Registry)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (log == null)
            {
                return NotFound();
            }

            return View(log);
        }

        // GET: Logs/Create
        public IActionResult Create()
        {
            ViewData["DnsServerId"] = new SelectList(_context.DnsServers, "Id", "AuthToken");
            ViewData["DomainId"] = new SelectList(_context.Domains, "Id", "Name");
            ViewData["JobId"] = new SelectList(_context.Jobs, "Id", "Id");
            ViewData["RegistryId"] = new SelectList(_context.Registries, "Id", "Name");
            return View();
        }

        // POST: Logs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Message,RawMessage,CreatedAt,LogType,DomainId,DnsServerId,RegistryId,JobId")] Log log)
        {
            if (ModelState.IsValid)
            {
                _context.Add(log);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DnsServerId"] = new SelectList(_context.DnsServers, "Id", "AuthToken", log.DnsServerId);
            ViewData["DomainId"] = new SelectList(_context.Domains, "Id", "Name", log.DomainId);
            ViewData["JobId"] = new SelectList(_context.Jobs, "Id", "Id", log.JobId);
            ViewData["RegistryId"] = new SelectList(_context.Registries, "Id", "Name", log.RegistryId);
            return View(log);
        }

        // GET: Logs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var log = await _context.Logs.FindAsync(id);
            if (log == null)
            {
                return NotFound();
            }
            ViewData["DnsServerId"] = new SelectList(_context.DnsServers, "Id", "AuthToken", log.DnsServerId);
            ViewData["DomainId"] = new SelectList(_context.Domains, "Id", "Name", log.DomainId);
            ViewData["JobId"] = new SelectList(_context.Jobs, "Id", "Id", log.JobId);
            ViewData["RegistryId"] = new SelectList(_context.Registries, "Id", "Name", log.RegistryId);
            return View(log);
        }

        // POST: Logs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Message,RawMessage,CreatedAt,LogType,DomainId,DnsServerId,RegistryId,JobId")] Log log)
        {
            if (id != log.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(log);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LogExists(log.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["DnsServerId"] = new SelectList(_context.DnsServers, "Id", "AuthToken", log.DnsServerId);
            ViewData["DomainId"] = new SelectList(_context.Domains, "Id", "Name", log.DomainId);
            ViewData["JobId"] = new SelectList(_context.Jobs, "Id", "Id", log.JobId);
            ViewData["RegistryId"] = new SelectList(_context.Registries, "Id", "Name", log.RegistryId);
            return View(log);
        }

        // GET: Logs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var log = await _context.Logs
                .Include(l => l.DnsServer)
                .Include(l => l.Domain)
                .Include(l => l.Job)
                .Include(l => l.Registry)
                .Include(l => l.LogType)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (log == null)
            {
                return NotFound();
            }

            return View(log);
        }

        // POST: Logs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var log = await _context.Logs.FindAsync(id);
            _context.Logs.Remove(log);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LogExists(int id)
        {
            return _context.Logs.Any(e => e.Id == id);
        }
    }
}
