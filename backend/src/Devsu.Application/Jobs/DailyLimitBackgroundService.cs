using Devsu.Application.Services.Accounts;
using Devsu.Application.Services.Users;
using Domain.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Devsu.Application.Jobs;

public class DailyLimitBackgroundService : BackgroundService
{
    private readonly ILogger<DailyLimitBackgroundService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly JobOption _option;

    public DailyLimitBackgroundService(ILogger<DailyLimitBackgroundService> logger,
        IServiceProvider serviceProvider, IOptions<JobOption> option)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _option = option.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        =>
            await Task.Run(async () =>
            {
                if (_option is not { Enabled: true })
                    return;

                try
                {
                    while (!stoppingToken.IsCancellationRequested)
                    {
                        using var scope = _serviceProvider.CreateScope();
                        var service = scope.ServiceProvider.GetRequiredService<IAccountService>();

                        _logger.LogInformation("check started");


                        await service.CheckDailyLimitAsync(stoppingToken)
                            .ConfigureAwait(false);
                        
                       
                        _logger.LogInformation("completed successfully");

                        await Task.Delay(TimeSpan.FromMinutes(_option.IntervalInMinutes),
                            stoppingToken);
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error in DailyLimitBackground {Message}", e.Message);
                }
            }, stoppingToken);
}