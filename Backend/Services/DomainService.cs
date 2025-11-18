using System;
using System.Linq;
using System.Threading.Tasks;
using Backend.Business;
using Backend.Data;
using Backend.Models;
using Microsoft.Extensions.Configuration;
using Providers;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services;

// Domain service specific exception to carry user-friendly error messages
public class DomainServiceException : Exception
{
    public DomainServiceException(string message) : base(message) {}
    public DomainServiceException(string message, Exception inner) : base(message, inner) {}
}

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
        if (domain == null)
        {
            throw new DomainServiceException("Domain payload was empty.");
        }

        // Normalize
        domain.Name = domain.Name?.Trim().ToLowerInvariant();

        // Business validations
        if (string.IsNullOrWhiteSpace(domain.Name))
        {
            throw new DomainServiceException("Domain name is required.");
        }

        if (!IsValidDomain(domain.Name))
        {
            throw new DomainServiceException("Please enter a valid domain name (e.g., example.com).");
        }

        // Duplicate
        if (await _context.Domains.AnyAsync(d => d.Name == domain.Name))
        {
            throw new DomainServiceException("This domain already exists.");
        }

        // Require DNS server selection and verify existence
        if (domain.DnsServerId == 0)
        {
            throw new DomainServiceException("Please choose a valid DNS Server.");
        }

        var dnsServer = await _context.DnsServers.FirstOrDefaultAsync(b => b.Id == domain.DnsServerId);
        if (dnsServer == null)
        {
            throw new DomainServiceException("Selected DNS Server was not found.");
        }

        IDnsProvider provider;
        try
        {
            provider = _providerDecider.DnsProvider(dnsServer);
        }
        catch (Exception ex)
        {
            throw new DomainServiceException("Connection with the DNS Server failed.", ex);
        }

        var createResponse = provider.CreateZone(domain.Name);

        if (!createResponse.Success)
        {
            var err = string.IsNullOrWhiteSpace(createResponse.Error) ? "Failed to create zone on DNS server." : createResponse.Error;
            throw new DomainServiceException(err);
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
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            // In case a unique index exists or a race condition causes a duplicate
            if (await _context.Domains.AnyAsync(d => d.Name == domain.Name))
            {
                throw new DomainServiceException("This domain already exists.", ex);
            }
            throw;
        }

        return domain;
    }

    private static bool IsValidDomain(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return false;
        bool hasDot = name.Contains('.');
        bool allowedChars = name.All(c => char.IsLetterOrDigit(c) || c == '-' || c == '.');
        bool noEdgeDots = !(name.StartsWith('.') || name.EndsWith('.'));
        bool noConsecutiveDots = !name.Contains("..");
        return hasDot && allowedChars && noEdgeDots && noConsecutiveDots;
    }
}