namespace Backend.Models
{
    public enum JobStep
    {
        SignDomainAtDnsServer = 1,
        UploadKeyToRegistry = 2,
        SignDomainCompleted = 3,
        DeleteAllKeysFromRegistry = 4,
        DeleteAllKeysFromDnsServer = 5,
        DeleteOldKeyFromRegistry = 6,
        DeleteOldKeyFromDnsServer = 7
    }
}
