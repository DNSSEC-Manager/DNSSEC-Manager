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

        // Voeg DnsServer toe
        _context.Add(dnsServer);

        // Maak de Job aan
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

        // Check if all three variables are filled
        if (string.IsNullOrWhiteSpace(name) || 
            string.IsNullOrWhiteSpace(baseUrl) || 
            string.IsNullOrWhiteSpace(authToken))
        {
            Console.WriteLine("DNS environment variables not fully configured — skipping creation.");
            return null;
        }

        // Check if DNS Server is already created in the database
        var existingServer = await _context.DnsServers
            .FirstOrDefaultAsync(x => x.BaseUrl == baseUrl || x.Name == name);

        if (existingServer != null)
        {
            Console.WriteLine($"DnsServer '{name}' already exists — skipping creation.");
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
        //Console.WriteLine("Creating DNS server...");
        var server = await CreateDnsServerAsync(dnsServer);
        Console.WriteLine($"DNS Server '{name}' added from environment variables.");
        
        //  _context.Attach(server);
        // Console.WriteLine($"Created DNS server with ID: {server.Id}");
        //
        // Adding example nameservers for this DNS Server
        /*var nameServerGroup = new NameServerGroup
        {
            Name = "ns1+ns2.example.com",
            DnsServerId = server.Id,
            DnsServer = server,
            NameServers =
            {
                new NameServer { Name = "ns1.example.com" },
                new NameServer { Name = "ns2.example.com" }
            }
        };

        server.NameServerGroups.Add(nameServerGroup);
        await _context.SaveChangesAsync();*/
        
        return server;
    }
}