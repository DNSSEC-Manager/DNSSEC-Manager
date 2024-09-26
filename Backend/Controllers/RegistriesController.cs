using System.Linq;
using System.Threading.Tasks;
using Backend.Business;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.Data;
using Backend.Models;
using Backend.ViewModels;
using Microsoft.AspNetCore.DataProtection;

namespace Backend.Controllers
{
    public class RegistriesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IUtilities _utilities;
        private readonly IProviderDecider _providerDecider;

        public RegistriesController(ApplicationDbContext context, IUtilities utilities, IProviderDecider providerDecider)
        {
            _context = context;
            _providerDecider = providerDecider;
            _utilities = utilities;
        }

        [HttpPost]
        public string TestConnection(string name, string url, string port, string username, string password, RegistryType registryType, int? id)
        {
            if (string.IsNullOrEmpty(password))
            {
                if (id != null)
                {
                    var registryInDb = _context.Registries.Find(id);
                    password = _utilities.Unprotect(registryInDb.Password);
                }
            }

            var registry = new Registry
            {
                RegistryType = registryType,
                Url = url,
                Username = username,
                Password = _utilities.Protect(password),
                Port = port
            };

            var registryProvider = _providerDecider.InitializeRegistryProvider(registry);
            var connected = registryProvider.CheckConnection();
            registryProvider.Close();

            return connected.Success ? "success" : connected.Error;
        }


        [HttpPost]
        public string TestConnectionId(int id)
        {
            var registry = _context.Registries.FirstOrDefault(m => m.Id == id);

            if (registry == null)
            {
                return "404";
            }

            //var registryResult = registry.Result;

            var registryProvider = _providerDecider.InitializeRegistryProvider(registry);

            var connected = registryProvider.CheckConnection();
            registryProvider.Close();

            return connected.Success ? "success" : connected.Error;
        }


        // GET: Registries
        public async Task<IActionResult> Index()
        {
            return View(await _context.Registries.Include(d => d.Domains).ToListAsync());
        }

        // GET: Registries/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var registry = await _context.Registries
                .FirstOrDefaultAsync(m => m.Id == id);
            if (registry == null)
            {
                return NotFound();
            }

            var tlds = _context.TopLevelDomainRegistries.Include(b => b.Registry).Include(b => b.TopLevelDomain)
                .Where(b => b.RegistryId == registry.Id).Select(b => b.TopLevelDomain).ToList();

            var vm = new RegistryDetailsViewModel
            {
                Id = registry.Id,
                Url = registry.Url,
                Port = registry.Port,
                Name = registry.Name,
                Username = registry.Username,
                RegistryType = registry.RegistryType,
                TopLevelDomains = tlds
            };

            return View(vm);
        }

        [HttpPost]
        public string DeleteTldRegistry(int tldId, int registryId)
        {
            var tldReg = _context.TopLevelDomainRegistries.FirstOrDefault(b =>
                b.TopLevelDomainId == tldId && b.RegistryId == registryId);

            if (tldReg == null)
            {
                return "The TLD is not found";
            }

            _context.Remove(tldReg);
            _context.SaveChanges();

            return "success";
        }

        public string AddTldRegistry(int registryId, string newTld)
        {
            var registry = _context.Registries.FirstOrDefault(b => b.Id == registryId);
            if (registry == null)
            {
                return "No registry found";
            }

            if (string.IsNullOrEmpty(newTld))
            {
                return "Please enter the topleveldomain";
            }

            if (newTld.StartsWith("."))
            {
                newTld = newTld.TrimStart(new char[] { '.' });
            }

            var tld = _context.TopLevelDomains.FirstOrDefault(b => b.Tld == newTld.ToLower());
            if (tld == null)
            {
                tld = new TopLevelDomain { Tld = newTld.ToLower() };
                _context.Add(tld);
                _context.SaveChanges();
            }

            var tldRegistry = new TopLevelDomainRegistry
            {
                RegistryId = registryId,
                TopLevelDomainId = tld.Id
            };

            _context.TopLevelDomainRegistries.Add(tldRegistry);
            _context.SaveChanges();

            return "success";
        }

        // GET: Registries/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Registries/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Url,Port,Username,Password,RegistryType")] Registry registry)
        {
            if (ModelState.IsValid)
            {
                var tempAuth = registry.Password;
                registry.Password = _utilities.Protect(tempAuth);
                _context.Add(registry);
                await _context.SaveChangesAsync();

                var domains = _context.Domains.Where(b => b.CustomRegistryId == null).ToList();
                foreach (var domain in domains)
                {
                    domain.LastChecked = null;
                }

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(registry);
        }

        // GET: Registries/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var registry = await _context.Registries.FindAsync(id);
            if (registry == null)
            {
                return NotFound();
            }

            var vm = new EditRegistryViewModel
            {
                Id = registry.Id,
                Name = registry.Name,
                Url = registry.Url,
                Port = registry.Port,
                RegistryType = registry.RegistryType,
                Username = registry.Username
            };

            return View(vm);
        }

        // POST: Registries/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Url,Port,Username,Password,IsDefault,RegistryType")] Registry registry)
        {
            if (id != registry.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var registryInDb = _context.Registries.Find(id);

                    registryInDb.Name = registry.Name;
                    registryInDb.Url = registry.Url;
                    registryInDb.Username = registry.Username;

                    if (!string.IsNullOrWhiteSpace(registry.Password))
                    {
                        registryInDb.Password = _utilities.Protect(registry.Password);
                    }

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RegistryExists(registry.Id))
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
            return View(registry);
        }

        // GET: Registries/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var registry = await _context.Registries
                .FirstOrDefaultAsync(m => m.Id == id);
            if (registry == null)
            {
                return NotFound();
            }

            return View(registry);
        }


        // POST: Registries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // check if this Registry has a connection with a domain (CustomRegistryId) and set it to 0
            await _context.Domains.Where(x => x.CustomRegistryId == id).ForEachAsync(x => x.CustomRegistryId = null);

            // check if this Registry has connected TLDs and remove the connection
            var tldRegistry = await _context.TopLevelDomainRegistries.Where(x => x.RegistryId == id).ToListAsync();
            _context.TopLevelDomainRegistries.RemoveRange(tldRegistry);

            // check if this Registry has connected Log entries
            var logs = await _context.Logs.Where(x => x.RegistryId == id).ToListAsync();
            _context.Logs.RemoveRange(logs);

            var registry = await _context.Registries.FindAsync(id);
            _context.Registries.Remove(registry);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RegistryExists(int id)
        {
            return _context.Registries.Any(e => e.Id == id);
        }
    }
}
