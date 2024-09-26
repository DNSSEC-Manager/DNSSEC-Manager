using System;
using System.Collections.Generic;
using System.Linq;
using Backend.Business;
using Backend.Data;
using Backend.Models;
using Backend.Scheduler;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Providers;

namespace Backend.Controllers
{
    public class SchedulerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IUtilities _utilities;
        private readonly IProviderDecider _providerDecider;
        private readonly IGlobals _globals;

        public SchedulerController(ApplicationDbContext context, IUtilities utilities, IProviderDecider providerDecider, IGlobals globals)
        {
            _context = context;
            _utilities = utilities;
            _providerDecider = providerDecider;
            _globals = globals;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            var scheduler = new Scheduler.Scheduler(_context, _utilities, _providerDecider, _globals);
            scheduler.Run();
            return View();
        }
    }
}