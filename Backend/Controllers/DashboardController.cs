using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Backend.Models;
using Backend.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Backend.ViewModels;

namespace Backend.Controllers
{
    public class DashboardController : Controller
    {
        
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var signedDomains = _context.Domains.Count(b => b.SignMatch && !b.RemovedFromDnsServer && b.CustomRegistryId != null);
            var notSigned = _context.Domains.Count(b => !b.SignMatch && !b.RemovedFromDnsServer && b.CustomRegistryId != null);
            var removedFromDns = _context.Domains.Count(b => b.RemovedFromDnsServer);
            var active = _context.Domains.Count(b => !b.RemovedFromDnsServer && b.CustomRegistryId != null);
            var inActive = _context.Domains.Count(b => !b.RemovedFromDnsServer && b.CustomRegistryId == null);

            var vm = new DashboardViewModel
            {
                DomainCountTotal = _context.Domains.Count(),
                DomainCountRemovedFromDnsServer = _context.Domains.Count(x => x.RemovedFromDnsServer),
                DomainCountHasRegistry =
                    _context.Domains.Count(x => x.CustomRegistryId != null && x.CustomRegistryId != 0),
                DomainCountNameserversMatch =
                    _context.Domains.Count(x => x.NameServerGroupId != null && x.NameServerGroupId != 0),
                DomainCountDnssecSigned = _context.Domains.Count(x => x.SignMatch),
                TopLevelDomains = await _context.TopLevelDomains.Include(b => b.Domains)
                    .OrderByDescending(b => b.Domains.Count).Take(5).ToListAsync(),
                LastAddedDomains = await _context.Domains.OrderByDescending(d => d.CreatedAt).Take(5).ToListAsync(),
                SignedDomainsCount = signedDomains,
                NotSignedDomainsCount = notSigned,
                ActiveDomainsCount = active,
                InActiveDomainsCount = inActive,
                RemovedDomainsCount = removedFromDns,
            };
            return View(vm);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
