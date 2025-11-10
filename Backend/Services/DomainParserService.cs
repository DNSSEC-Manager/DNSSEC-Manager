using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Nager.PublicSuffix;
using Nager.PublicSuffix.RuleProviders;

namespace Backend.Business
{
    public interface IDomainParserService
    {
        DomainInfo Parse(string domain);
        bool IsInitialized { get; }
    }

    public class DomainParserService : IDomainParserService, IHostedService
    {
        private DomainParser _domainParser;
        private readonly SemaphoreSlim _initLock = new SemaphoreSlim(1, 1);
        private bool _isInitialized = false;

        public bool IsInitialized => _isInitialized;

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await InitializeAsync();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private async Task InitializeAsync()
        {
            await _initLock.WaitAsync();
            try
            {
                if (!_isInitialized)
                {
                    var ruleProvider = new SimpleHttpRuleProvider();
                    await ruleProvider.BuildAsync();
                    _domainParser = new DomainParser(ruleProvider);
                    _isInitialized = true;
                }
            }
            finally
            {
                _initLock.Release();
            }
        }

        public DomainInfo Parse(string domain)
        {
            if (!_isInitialized)
            {
                throw new InvalidOperationException("DomainParser is not initialized. Please wait for application startup to complete.");
            }

            return _domainParser.Parse(domain);
        }
    }
}
