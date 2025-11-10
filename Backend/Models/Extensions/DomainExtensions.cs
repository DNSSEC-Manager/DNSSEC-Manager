using Providers.Dto;

namespace Backend.Models.Extensions
{
    public static class DomainExtensions
    {
        /// <summary>
        /// Converts a Domain entity to a DomainData DTO for use with providers.
        /// </summary>
        public static DomainData ToDomainData(this Domain domain)
        {
            return new DomainData
            {
                FullName = domain.Name,
                NameWithoutTld = domain.NameWithoutTld,
                Tld = domain.TopLevelDomain?.Tld
            };
        }
    }
}
