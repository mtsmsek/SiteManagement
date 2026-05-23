using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SiteManagement.Application.Abstractions.Events;

namespace SiteManagement.Infrastructure.Events;

/// <summary>
/// Drives <see cref="IOutboxProcessor"/> on a timer. Resolves a fresh scope per
/// pass (the processor depends on the scoped DbContext) and never lets a failed
/// pass kill the loop — it logs and waits for the next tick.
/// </summary>
public sealed class OutboxBackgroundService(
    IServiceScopeFactory scopeFactory,
    IOptions<OutboxOptions> options,
    ILogger<OutboxBackgroundService> logger)
    : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
    private readonly ILogger<OutboxBackgroundService> _logger = logger;
    private readonly TimeSpan _interval = TimeSpan.FromSeconds(Math.Max(1, options.Value.PollSeconds));

    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(_interval, stoppingToken);

                using var scope = _scopeFactory.CreateScope();
                var processor = scope.ServiceProvider.GetRequiredService<IOutboxProcessor>();
                await processor.ProcessPendingAsync(stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Outbox processing pass failed; retrying next cycle.");
            }
        }
    }
}
