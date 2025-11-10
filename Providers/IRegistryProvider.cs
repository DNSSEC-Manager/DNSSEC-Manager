using Providers.Dto;

namespace Providers
{
    public interface IRegistryProvider
    {
        ProviderResponse CheckConnection();
        
        //bool DomainExists(string domainname);
        bool DomainExists(DomainData domainData);
        
        //RegistryDomainInfo GetDomainInfo(string domainname);
        RegistryDomainInfo GetDomainInfo(DomainData domainData);
        
        //ProviderResponse Sign(string domainname, string flags, string algorithm, string publicKey, string keyTag = "");
        ProviderResponse Sign(DomainData domainData, string flags, string algorithm, string publicKey, string keyTag = "");

        //ProviderResponse DeleteKey(string domainname, string flags, string algorithm, string publicKey);
        ProviderResponse DeleteKey(DomainData domainData, string flags, string algorithm, string publicKey);

        void Close();
    }
}
