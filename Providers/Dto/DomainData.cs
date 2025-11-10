namespace Providers.Dto
{
    /// <summary>
    /// Represents domain information needed by registry providers.
    /// This is a lightweight DTO to avoid coupling the provider library to the backend models.
    /// </summary>
    public class DomainData
    {
        /// <summary>
        /// Full domain name (e.g., "example.com")
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Domain name without TLD (e.g., "example")
        /// </summary>
        public string NameWithoutTld { get; set; }

        /// <summary>
        /// Top-level domain (e.g., "com", "co.uk")
        /// </summary>
        public string Tld { get; set; }

        /// <summary>
        /// Creates a DomainData instance from a full domain name string.
        /// Requires IDomainParserService to parse the domain.
        /// </summary>
        // public static DomainData FromString(string fullDomainName, IDomainParserService parserService)
        // {
        //     var parsed = parserService.Parse(fullDomainName);
        //     
        //     if (parsed == null)
        //     {
        //         return null;
        //     }
        //
        //     return new DomainData
        //     {
        //         FullName = fullDomainName,
        //         NameWithoutTld = parsed.Domain,
        //         Tld = parsed.TopLevelDomain
        //     };
        // }
    }
}
