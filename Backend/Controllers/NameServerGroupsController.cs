using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.Data;
using Backend.Models;

namespace Backend.Controllers
{
    public class NameServerGroupsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NameServerGroupsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public string CreateGroup(string name, int dnsServerId, List<string> nameServers)
        {
            nameServers = nameServers.Where(b => !string.IsNullOrEmpty(b)).ToList();

            if (nameServers.Count < 2)
            {
                return "There should at least 2 nameservers";
            }

            if (string.IsNullOrEmpty(name))
            {
                return "Name is not filled in";
            }

            var nameserverNameCheck = _context.NameServerGroups.FirstOrDefault(b => b.Name == name);
            if (nameserverNameCheck != null)
            {
                return "There is already a nameserver group with the same name";
            }

            // Check if there are already nameservergroups with the same nameservers
            var nameserverGroups = _context.NameServerGroups.Include(b => b.NameServers).ToList();
            foreach (var nameserverGroup in nameserverGroups)
            {
                var nameserverGroupNameservers = nameserverGroup.NameServers.Select(nameserverGroupNameserver => nameserverGroupNameserver.Name).ToList();

                if (nameServers.ToHashSet().SetEquals(nameserverGroupNameservers))
                {
                    return "There is already a nameservergroup with the same nameservers";
                }
            }

            var nameServerGroup = new NameServerGroup
            {
                DnsServerId = dnsServerId,
                Name = name
            };

            _context.Add(nameServerGroup);
            _context.SaveChanges();

            var nameServersList = new List<NameServer>();

            foreach (var nameServer in nameServers)
            {
                if (string.IsNullOrEmpty(nameServer))
                {
                    continue;
                }
                var newNameServer = new NameServer
                {
                    Name = nameServer,
                    NameServerGroupId = nameServerGroup.Id
                };
                nameServersList.Add(newNameServer);
            }
            _context.AddRange(nameServersList);
            _context.SaveChanges();
            var domains = _context.Domains.Include(b => b.DnsServer).Where(b => b.NameServerGroup == null && b.CustomRegistryId != null && b.DnsServerId == dnsServerId).ToList();
            foreach (var domain in domains)
            {
                domain.LastChecked = null;
            }
            _context.SaveChanges();
            return "success";
        }

        // POST: NameServerGroups/Delete/5
        [HttpPost]
        public string Delete(int id)
        {
            var nameServerGroup = _context.NameServerGroups.Include(b => b.Domains).Include(b => b.NameServers).FirstOrDefault(m => m.Id == id);
            if (nameServerGroup != null)
            {
                foreach (var domain in nameServerGroup.Domains)
                {
                    domain.NameServerGroupId = null;
                    domain.NameServerGroup = null;
                    domain.LastChecked = null;
                    domain.SignMatch = false;
                }

                _context.NameServers.RemoveRange(nameServerGroup.NameServers);
                _context.NameServerGroups.Remove(nameServerGroup);
                _context.SaveChanges();
            }
            else
            {
                return "Nameserver group could not be found";
            }
            return "success";
        }

    }
}
