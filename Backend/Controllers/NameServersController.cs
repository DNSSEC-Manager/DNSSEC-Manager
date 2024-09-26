using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Backend.Data;
using Backend.Models;
using Microsoft.CodeAnalysis;

namespace Backend.Controllers
{
    public class NameServersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NameServersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: NameServers
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.NameServerGroups
                .Include(n => n.DnsServer)
                .Include(n => n.NameServers)
                .Include(n => n.Domains);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: NameServers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nameServerGroup = await _context.NameServerGroups
                .Include(n => n.DnsServer)
                .Include(d => d.Domains)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (nameServerGroup == null)
            {
                return NotFound();
            }

            return View(nameServerGroup);
        }

        // GET: NameServers/Create
        public IActionResult Create()
        {
            ViewData["DnsServerId"] = new SelectList(_context.DnsServers, "Id", "Name");
            return View();
        }

        // POST: NameServers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,DnsServerId")] NameServerGroup nameServerGroup)
        {
            if (ModelState.IsValid)
            {
                _context.Add(nameServerGroup);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DnsServerId"] = new SelectList(_context.DnsServers, "Id", "Name", nameServerGroup.DnsServerId);
            return View(nameServerGroup);
        }

        // GET: NameServers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nameServerGroup = await _context.NameServerGroups.FindAsync(id);
            if (nameServerGroup == null)
            {
                return NotFound();
            }
            var selectListItems = new SelectList(_context.DnsServers, "Id", "Name", nameServerGroup.DnsServerId);
            //var defaultItem = new SelectListItem { Text = "test", Value = "" };
            //selectListItems.Prepend<SelectListItem>( defaultItem );
            ViewData["DnsServerId"] = selectListItems;
            //TODO add select option for "not connected to a DNS Server"
            return View(nameServerGroup);
        }

        // POST: NameServers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,DnsServerId")] NameServerGroup nameServerGroup)
        {
            if (id != nameServerGroup.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(nameServerGroup);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NameServerGroupExists(nameServerGroup.Id))
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
            ViewData["DnsServerId"] = new SelectList(_context.DnsServers, "Id", "Name", nameServerGroup.DnsServerId);
            return View(nameServerGroup);
        }

        // GET: NameServers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nameServerGroup = await _context.NameServerGroups
                .Include(n => n.DnsServer)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (nameServerGroup == null)
            {
                return NotFound();
            }

            return View(nameServerGroup);
        }

        // POST: NameServers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var nameServerGroup = await _context.NameServerGroups.FindAsync(id);
            _context.NameServerGroups.Remove(nameServerGroup);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NameServerGroupExists(int id)
        {
            return _context.NameServerGroups.Any(e => e.Id == id);
        }
    }
}
