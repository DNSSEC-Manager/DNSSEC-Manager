using System;
using System.Threading;
using System.Threading.Tasks;
using Backend.Business;
using Backend.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Backend.Scheduler
{
    public sealed class JobSchedulerService : BackgroundService
    {
        private readonly IServiceProvider _provider;
        private readonly ILogger<JobSchedulerService> _logger;
        private readonly TimeSpan _interval;

        public JobSchedulerService(IServiceProvider provider, ILogger<JobSchedulerService> logger)
        {
            _provider = provider;
            _logger = logger;
            // Default interval 10 seconds; can be made configurable via options if needed
            _interval = TimeSpan.FromSeconds(10);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("JobSchedulerService started with interval {IntervalSeconds}s", _interval.TotalSeconds);
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _provider.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var utilities = scope.ServiceProvider.GetRequiredService<IUtilities>();
                    var decider = scope.ServiceProvider.GetRequiredService<IProviderDecider>();
                    var globals = scope.ServiceProvider.GetRequiredService<IGlobals>();
                    var loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
                    var schedulerLogger = loggerFactory.CreateLogger<Scheduler>();

                    var scheduler = new Scheduler(db, utilities, decider, globals, schedulerLogger, loggerFactory);
                    await scheduler.RunOnceAsync(stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    // normal shutdown
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Scheduler tick failed");
                }

                try
                {
                    await Task.Delay(_interval, stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
            _logger.LogInformation("JobSchedulerService stopped");
        }
    }
}
