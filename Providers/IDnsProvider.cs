using System.Collections.Generic;
using Providers.Dto;

namespace Providers
{
    public interface IDnsProvider
    {
        DnsZoneCryptokey Sign(string id, string algorithm, int bits);

        ProviderResponse UnSign(string id, string pubKey);

        ProviderResponse CheckConnection();

        List<string> GetZones();

        DnsZoneInfo GetZoneInfo(string id);

        List<DnsRrset> GetRrsets(string id);

        ProviderResponse DeleteRrset(string id, string name, string type, string content);

        ProviderResponse CreateRrset(string id, string name, int ttl, string type, string content, bool replace = false);

        ProviderResponse EditRrset(string id, string oldName, int oldTtl, string oldType, string oldContent, string newName, int newTtl, string newContent, bool replace = false);

        int GetTtl(string id);

        ProviderResponse NotifyZone(string id);

        ProviderResponse RectifyZone(string id);

        ProviderResponse RetrieveZone(string id);

        ProviderResponse VerifyZone(string id);

        ProviderResponse ChangeKind(string id, string kind);

        ProviderResponse EditMasterIps(string id, List<string> ipList);

        ProviderResponse DeleteZone(string id);

        ProviderResponse CreateZone(string id);
    }
}




