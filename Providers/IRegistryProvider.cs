using Providers.Dto;

namespace Providers
{
    public interface IRegistryProvider
    {
        ProviderResponse CheckConnection();

        bool DomainExists(string domainname);

        RegistryDomainInfo GetDomainInfo(string domainname);

        ProviderResponse Sign(string domainname, string flags, string algorithm, string publicKey, string keyTag = "");

        ProviderResponse DeleteKey(string domainname, string flags, string algorithm, string publicKey);

        void Close();
    }
}
