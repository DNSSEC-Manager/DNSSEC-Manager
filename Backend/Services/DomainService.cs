using System;
using System.Linq;
using System.Threading.Tasks;
using Backend.Business;
using Backend.Data;
using Backend.Models;
using Microsoft.Extensions.Configuration;
using Providers;

namespace Backend.Services;

public interface IDomainService
{
    Task<Domain> CreateDomainAsync(Domain domain);
}

public class DomainService : IDomainService
{
    private readonly ApplicationDbContext _context;
    private readonly IProviderDecider _providerDecider;
    private readonly IUtilities _utilities;
    private readonly IConfiguration _configuration;
    

    public DomainService(ApplicationDbContext context, IUtilities utilities, IConfiguration configuration, IProviderDecider providerDecider)
    {
        _context = context;
        _utilities = utilities;
        _configuration = configuration;
        _providerDecider = providerDecider;
    }

    public async Task<Domain> CreateDomainAsync(Domain domain)
    {
        domain.Name = domain.Name.ToLower();
        var dnsServer = _context.DnsServers.FirstOrDefault(b => b.Id == domain.DnsServerId);

        IDnsProvider provider;
        try
        {
            provider = _providerDecider.DnsProvider(dnsServer);
        }
        catch (Exception)
        {
            //return "Connection with the DNS Server failed.";
            //return Task.FromResult<Domain>(null);
            return null;
        }

        var createResponse = provider.CreateZone(domain.Name);

        if (!createResponse.Success)
        {
            //return createResponse.Error;
            //return Task.FromResult<Domain>(null);
            return null;
        }

        // var newDomain = new Domain
        // {
        //     Name = domain.Name,
        //     CreatedAt = DateTime.Now,
        //     DnsServerId = domain.DnsServerId,
        //     TopLevelDomainId = _utilities.GetTldId(domain.Name),
        //     Ttl = provider.GetTtl(domain.Name)
        // };
        
        domain.CreatedAt = DateTime.UtcNow;
        domain.TopLevelDomainId = _utilities.GetTldId(domain.Name);
        domain.Ttl = provider.GetTtl(domain.Name);
        _context.Add(domain);
        _context.SaveChanges();

        //return "success: " + newDomain.Id;
        return domain;
    }
}