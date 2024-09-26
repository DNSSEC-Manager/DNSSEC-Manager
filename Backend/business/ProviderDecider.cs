using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using Backend.Data;
using Backend.Models;
using Microsoft.AspNetCore.DataProtection;
using Providers;

namespace Backend.Business
{
    public interface IProviderDecider
    {
        IRegistryProvider InitializeRegistryProvider(Registry registry);
        IDnsProvider DnsProvider(DnsServer dnsServer);
    }
    public class ProviderDecider : IProviderDecider
    {
        private readonly ApplicationDbContext _context;
        private readonly IUtilities _utilities;

        public ProviderDecider(ApplicationDbContext context, IUtilities utilities)
        {
            _context = context;
            _utilities = utilities;
        }

        public IRegistryProvider InitializeRegistryProvider(Registry registry)
        {
            try
            {
                switch (registry.RegistryType)
                {
                    case RegistryType.Sidn:
                        return new Sidn(registry.Username, _utilities.Unprotect(registry.Password), registry.Url, Convert.ToInt32(registry.Port));

                    case RegistryType.Openprovider:
                        return new Openprovider(registry.Url, registry.Username, _utilities.Unprotect(registry.Password));

                    case RegistryType.Transip:
                        return new Transip(registry.Url, registry.Username, _utilities.Unprotect(registry.Password));

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            catch (Exception e)
            {
                _context.Logs.Add(new Log
                {
                    Message = "Failed initializing registry " + registry.Name + " (" + e.Message + ")",
                    LogType = LogType.Error,
                    RegistryId = registry.Id,
                    CreatedAt = DateTime.Now
                });

                _context.SaveChanges();
            }
            
            return null;
        }

        public IDnsProvider DnsProvider(DnsServer dnsServer)
        {
            IDnsProvider dnsProvider;

            switch (dnsServer.ServerType)
            {
                case DnsServerType.PowerDNS:
                    dnsProvider = new PowerDns(dnsServer.BaseUrl, _utilities.Unprotect(dnsServer.AuthToken));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return dnsProvider;
        }
    }
}
