using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Backend.Business;
using Backend.Data;
using Backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Backend.Services;

public class DnsServerService : IDnsServerService
{
    private readonly ApplicationDbContext _context;
    private readonly IUtilities _utilities;
    private readonly IConfiguration _configuration;

    public DnsServerService(ApplicationDbContext context, IUtilities utilities, IConfiguration configuration)
    {
        _context = context;
        _utilities = utilities;
        _configuration = configuration;
    }

    public async Task<DnsServer> CreateDnsServerAsync(DnsServer dnsServer)
    {
        // Protect the AuthToken
        dnsServer.AuthToken = _utilities.Protect(dnsServer.AuthToken);

        // Add DnsServer
        _context.Add(dnsServer);

        // Create Permanent Job
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

        return dnsServer;
    }

    public async Task<DnsServer> CreateDnsServerFromEnvironmentAsync()
    {
        var name = Environment.GetEnvironmentVariable("DNS_SERVER_NAME");
        var baseUrl = Environment.GetEnvironmentVariable("DNS_SERVER_API_URL");
        var authToken = Environment.GetEnvironmentVariable("DNS_SERVER_API_KEY");
        var ns1ev = Environment.GetEnvironmentVariable("DNS_SERVER_NS1");
        var ns2ev = Environment.GetEnvironmentVariable("DNS_SERVER_NS2");

        // Check if all variables are filled
        if (string.IsNullOrWhiteSpace(name) || 
            string.IsNullOrWhiteSpace(baseUrl) || 
            string.IsNullOrWhiteSpace(authToken) ||
            string.IsNullOrWhiteSpace(ns1ev) ||
            string.IsNullOrWhiteSpace(ns2ev))
        {
            //Console.WriteLine("DNS environment variables not fully configured — skipping creation.");
            return null;
        }

        // Check if DNS Server is already created in the database
        var existingServer = await _context.DnsServers
            .FirstOrDefaultAsync(x => x.BaseUrl == baseUrl || x.Name == name);

        if (existingServer != null)
        {
            //Console.WriteLine($"DnsServer '{name}' already exists — skipping creation.");
            return existingServer;
        }
        
        var dnsServer = new DnsServer
        {
            Name = Environment.GetEnvironmentVariable("DNS_SERVER_NAME"),
            BaseUrl = Environment.GetEnvironmentVariable("DNS_SERVER_API_URL"),
            ServerType = DnsServerType.PowerDNS,
            AuthToken = Environment.GetEnvironmentVariable("DNS_SERVER_API_KEY")
        };
        
        // Creating DNS Server
        var server = await CreateDnsServerAsync(dnsServer);
        Console.WriteLine($"DNS Server '{name}' added from environment variables.");
         _context.Attach(server);
        
        var nameServerGroup = new NameServerGroup
        {
            Name = "default",
            DnsServer = server
        };

        var ns1 = new NameServer { Name = ns1ev, NameServerGroup = nameServerGroup };
        var ns2 = new NameServer { Name = ns2ev, NameServerGroup = nameServerGroup };
        _context.NameServerGroups.Add(nameServerGroup);
        _context.NameServers.AddRange(ns1, ns2);
        await _context.SaveChangesAsync();
        Console.WriteLine("Default nameservers added");
        return server;
    }
}
