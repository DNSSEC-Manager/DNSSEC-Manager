using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.Business;
using Backend.Data;
using Backend.Models;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{

    public class SettingsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IUtilities _utilities;
        private readonly IProviderDecider _providerDecider;

        public SettingsController(ApplicationDbContext context, IUtilities utilities, IProviderDecider providerDecider)
        {
            _context = context;
            _utilities = utilities;
            _providerDecider = providerDecider;
        }

        public IActionResult Index()
        {
            ViewData["DomainChangesRunEvery"] = _utilities.ConfigToTimeObject(_context.Configs.Single(b => b.Key == "DomainChangesRunEvery"));
            ViewData["DomainChangesDefaultRuntime"] = _context.Configs.Single(b => b.Key == "DomainChangesDefaultRuntime").Value;
            ViewData["CheckAllDomainsRunEvery"] = _utilities.ConfigToTimeObject(_context.Configs.Single(b => b.Key == "CheckAllDomainsRunEvery"));
            ViewData["CheckAllDomainsReRunAfter"] = _utilities.ConfigToTimeObject(_context.Configs.Single(b => b.Key == "CheckAllDomainsReRunAfter"));
            ViewData["CheckAlldomainsPerRun"] = _context.Configs.Single(b => b.Key == "CheckAlldomainsPerRun").Value;
            ViewData["AutomaticSign"] = Convert.ToBoolean(_context.Configs.Single(b => b.Key == "AutomaticSign").Value);
            ViewData["AutomaticFix"] = Convert.ToBoolean(_context.Configs.Single(b => b.Key == "AutomaticFix").Value);
            ViewData["DomainSignFailRerun"] = _utilities.ConfigToTimeObject(_context.Configs.Single(b => b.Key == "DomainSignFailRerun"));
            ViewData["Algorithms"] = _context.Algorithms.Where(x => x.Disabled == false).ToList();
            ViewData["DefaultAlgorithm"] = Convert.ToInt32(_utilities.GetSetting("DefaultAlgorithm"));
            ViewData["AutomaticKeyRollover"] = Convert.ToBoolean(_context.Configs.Single(b => b.Key == "AutomaticKeyRollover").Value);
            ViewData["KeyRolloverTime"] = _utilities.MinutesToDays(Convert.ToInt32(_utilities.GetSetting("KeyRolloverTime")));
            ViewData["defaultTtl"] = Convert.ToInt32(_utilities.GetSetting("DefaultTtl")) / 3600;


            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public string SaveChanges(string automaticFix, string automaticSign, string automaticKeyRollover, int checkAllDomainsReRunAfterHours, int checkAllDomainsReRunAfterMinutes, 
            int checkAllDomainsRunEveryHours, int checkAllDomainsRunEveryMinutes, int domainChangesRunEveryHours, int domainChangesRunEveryMinutes, 
            int domainSignFailRerunHours, int domainSignFailRerunMinutes, string defaultAlgorithm, int keyRolloverTime, int defaultTtl)
        {
            _context.Configs.Single(b => b.Key == "AutomaticSign").Value = automaticSign;
            _context.Configs.Single(b => b.Key == "AutomaticFix").Value = automaticFix;
            _context.Configs.Single(b => b.Key == "AutomaticKeyRollover").Value = automaticKeyRollover;
            _context.Configs.Single(b => b.Key == "KeyRolloverTime").Value = _utilities.DaysToMinutes(keyRolloverTime).ToString();
            _context.Configs.Single(b => b.Key == "DomainChangesRunEvery").Value =
                _utilities.HoursMinutesToMs(domainChangesRunEveryHours, domainChangesRunEveryMinutes);
            _context.Configs.Single(b => b.Key == "CheckAllDomainsRunEvery").Value =
                _utilities.HoursMinutesToMs(checkAllDomainsRunEveryHours, checkAllDomainsRunEveryMinutes);
            _context.Configs.Single(b => b.Key == "CheckAllDomainsReRunAfter").Value =
                _utilities.HoursMinutesToMs(checkAllDomainsReRunAfterHours, checkAllDomainsReRunAfterMinutes);
            _context.Configs.Single(b => b.Key == "DomainSignFailRerun").Value =
                _utilities.HoursMinutesToMs(domainSignFailRerunHours, domainSignFailRerunMinutes);
            _context.Configs.Single(b => b.Key == "DefaultAlgorithm").Value = defaultAlgorithm;
            _context.Configs.Single(b => b.Key == "DefaultTtl").Value = (defaultTtl * 3600).ToString();
            try
            {
                _context.SaveChanges();
            }
            catch (Exception)
            {
                return "error";
            }

            return "success";

        }
    }
}