using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Backend.Business;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Backend.Data;
using Backend.Models;
using Microsoft.AspNetCore.DataProtection;
using Providers;
using Backend.ViewModels;

namespace Backend.Controllers
{
    public class DnsServersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IUtilities _utilities;
        private readonly IProviderDecider _providerDecider;

        public DnsServersController(ApplicationDbContext context, IUtilities utilities, IProviderDecider providerDecider)
        {
            _context = context;
            _utilities = utilities;
            _providerDecider = providerDecider;
        }

        [HttpPost]
        public string CheckConnectionLive(string url, string apiKey)
        {
            IDnsProvider dns = new PowerDns(url, apiKey);

            var error = dns.CheckConnection();

            return error.Success ? "success" : error.Error;
        }

        [HttpPost]
        public async Task<string> CheckConnection(int? id)
        {
            if (id == null)
            {
                return "Error: Not found";
            }

            var dnsServer = await _context.DnsServers.FirstOrDefaultAsync(m => m.Id == id);
            if (dnsServer == null)
            {
                return "Error: Not found ";
            }

            var dnsProvider = _providerDecider.DnsProvider(dnsServer);
            var error = dnsProvider.CheckConnection();

            return error.Success ? "success" : error.Error;
        }

        // GET: DnsServers
        public async Task<IActionResult> Index()
        {
            return View(await _context.DnsServers
                .Include(d => d.Domains)
                .ToListAsync());
        }

        // GET: DnsServers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dnsServer = await _context.DnsServers
                //.Include(d => d.Domains)
                .Include(d => d.NameServerGroups).ThenInclude(n => n.NameServers)
                .Include(d => d.NameServerGroups).ThenInclude(n => n.Domains)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dnsServer == null)
            {
                return NotFound();
            }

            return View(dnsServer);
        }

        // GET: DnsServers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: DnsServers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,BaseUrl,ServerType,AuthToken")] DnsServer dnsServer)
        {
            if (ModelState.IsValid)
            {
                dnsServer.AuthToken = _utilities.Protect(dnsServer.AuthToken);
                _context.Add(dnsServer);

                var newJob = new Job
                {
                    DnsServer = dnsServer,
                    DnsServerId = dnsServer.Id,
                    IsPermanent = true,
                    CreatedAt = DateTime.Now,
                    Task = JobName.CheckForDomainChanges,
                    UpdatedAt = DateTime.Now,
                    RunAfter = DateTime.Now
                };
                _context.Add(newJob);

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(dnsServer);
        }

        // GET: DnsServers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dnsServer = await _context.DnsServers.FindAsync(id);
            if (dnsServer == null)
            {
                return NotFound();
            }

            var vm = new EditDnsServerViewModel();
            vm.Id = dnsServer.Id;
            vm.Name = dnsServer.Name;
            vm.BaseUrl = dnsServer.BaseUrl;
            vm.ServerType = dnsServer.ServerType;

            return View(vm);
        }

        // POST: DnsServers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,BaseUrl,ServerType,AuthToken")] EditDnsServerViewModel dnsServer)
        {
            if (id != dnsServer.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var dnsServerInDatabase = _context.DnsServers.Find(id);

                    dnsServerInDatabase.Name = dnsServer.Name;
                    dnsServerInDatabase.BaseUrl = dnsServer.BaseUrl;

                    if (!string.IsNullOrWhiteSpace(dnsServer.AuthToken))
                        dnsServerInDatabase.AuthToken = _utilities.Protect(dnsServer.AuthToken);

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DnsServerExists(dnsServer.Id))
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
            return View(dnsServer);
        }

        // GET: DnsServers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dnsServer = await _context.DnsServers
                .Include(m => m.Domains)
                .Include(m => m.NameServerGroups)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dnsServer == null)
            {
                return NotFound();
            }

            return View(dnsServer);
        }

        // POST: DnsServers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // DnsServer contains: Jobs, Domains, Nameservergroups

            // Get all the jobs associated to the DNS Server and the Domains of the DNS Server
            var domainIds = _context.Domains.Where(b => b.DnsServerId == id).Select(b => b.Id).ToList();
            var jobs = _context.Jobs.Where(b => b.DnsServerId == id || domainIds.Contains(Convert.ToInt32(b.DomainId))).ToList();
            var cryptokeys = _context.Cryptokeys.Where(x => domainIds.Contains(Convert.ToInt32(x.Id))).ToList();

            // Set associated logs to null (otherwise we cannot delete the jobs)
            foreach (var job in jobs)
            {
                await _context.Logs.Where(x => x.JobId == job.Id).ForEachAsync(x => x.JobId = null);
            }

            _context.Cryptokeys.RemoveRange(cryptokeys);
            _context.Jobs.RemoveRange(jobs);

            var logs = await _context.Logs.Where(b => b.DnsServerId == id).ToListAsync();
            _context.Logs.RemoveRange(logs);
            await _context.SaveChangesAsync();

            var nameServerGroups = _context.NameServerGroups.Where(b => b.DnsServerId == id).ToList();
            var nameServerGroupIds = _context.NameServerGroups.Where(b => b.DnsServerId == id).Select(b => b.Id).ToList();
            var nameServers = _context.NameServers.Where(p => nameServerGroupIds.Contains(p.NameServerGroupId));
            _context.NameServers.RemoveRange(nameServers);
            _context.NameServerGroups.RemoveRange(nameServerGroups);

            var domains = _context.Domains.Where(b => b.DnsServerId == id).ToList();
            _context.Domains.RemoveRange(domains);

            var dnsServer = await _context.DnsServers.FindAsync(id);
            _context.DnsServers.Remove(dnsServer);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DnsServerExists(int id)
        {
            return _context.DnsServers.Any(e => e.Id == id);
        }
    }
}
