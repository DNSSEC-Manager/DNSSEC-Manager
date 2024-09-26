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

namespace Backend.Controllers
{
    public class TopLevelDomainsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TopLevelDomainsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: TopLevelDomains
        public async Task<IActionResult> Index()
        {
            var defaultAlgorithmId = Convert.ToInt32(_context.Configs.Single(b => b.Key == "DefaultAlgorithm").Value);
            var defaultAlgorithm = _context.Algorithms.Where(a => a.Id == defaultAlgorithmId).FirstOrDefault();
            if (defaultAlgorithm == null)
            {
                TempData["DefaultDnssecAlgorithm"] = "not set";
            } else
            {
                TempData["DefaultDnssecAlgorithm"] = defaultAlgorithm.Name + " ( " + defaultAlgorithm.Number + " )";
            }
            
            return View(await _context.TopLevelDomains
                .Include(b => b.Domains)
                .Include(b => b.Algorithm)
                .Include(b => b.TopLevelDomainRegistries).ThenInclude(x => x.Registry)
                .OrderByDescending(b => b.Domains.Count)
                .ToListAsync());
        }

        // GET: TopLevelDomains/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var topLevelDomain = await _context.TopLevelDomains.FindAsync(id);
            if (topLevelDomain == null)
            {
                return NotFound();
            }

            var vm = new EditTopLevelDomainViewModel
            {
                Id = topLevelDomain.Id,
                Tld = topLevelDomain.Tld,
                OverrideAlgorithmId = topLevelDomain.OverrideAlgorithmId,
                Algorithms = _context.Algorithms.Where(x => x.Disabled == false).ToList()
            };

            return View(vm);
        }

        // POST: TopLevelDomains/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Tld,OverrideAlgorithmId")] EditTopLevelDomainViewModel topLevelDomain)
        {
            if (id != topLevelDomain.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var topLevelDomainInDatabase = _context.TopLevelDomains.Find(id);

                    if (topLevelDomain.OverrideAlgorithmId == 0)
                    {
                        topLevelDomainInDatabase.OverrideAlgorithmId = null;
                    }
                    else
                    {
                        topLevelDomainInDatabase.OverrideAlgorithmId = topLevelDomain.OverrideAlgorithmId;
                    }
                    
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TopLevelDomainExists(topLevelDomain.Id))
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
            topLevelDomain.Algorithms = _context.Algorithms.Where(x => x.Disabled == false).ToList();
            return View(topLevelDomain);
        }

        // GET: TopLevelDomains/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var topLevelDomain = await _context.TopLevelDomains
                .FirstOrDefaultAsync(m => m.Id == id);
            if (topLevelDomain == null)
            {
                return NotFound();
            }

            return View(topLevelDomain);
        }

        // POST: TopLevelDomains/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var topLevelDomain = await _context.TopLevelDomains.FindAsync(id);
            _context.TopLevelDomains.Remove(topLevelDomain);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TopLevelDomainExists(int id)
        {
            return _context.TopLevelDomains.Any(e => e.Id == id);
        }
    }
}
