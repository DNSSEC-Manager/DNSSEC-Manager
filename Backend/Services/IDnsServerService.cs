using System.Threading.Tasks;
using Backend.Models;

namespace Backend.Services;

public interface IDnsServerService
{
    Task<DnsServer> CreateDnsServerAsync(DnsServer dnsServer);
    Task<DnsServer> CreateDnsServerFromEnvironmentAsync();
}