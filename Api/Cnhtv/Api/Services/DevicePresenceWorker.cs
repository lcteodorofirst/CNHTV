namespace Cnhtv.Api.Services;

using Cnhtv.Application.Common;
using Cnhtv.Infrastructure.DataContext;
using Microsoft.EntityFrameworkCore;

public sealed class DevicePresenceWorker(
    IServiceScopeFactory scopeFactory,
    ILogger<DevicePresenceWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(30));
        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                using var scope = scopeFactory.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<CnhtvDbContext>();
                var now = DateTime.UtcNow;
                var limit = now.AddSeconds(-DisplayDefaults.OfflineTimeoutSeconds);
                var staleDevices = await dbContext.TvDevices
                    .Where(x => x.IsOnline && x.LastSeenAtUtc < limit)
                    .ToListAsync(stoppingToken);

                if (staleDevices.Count == 0)
                    continue;

                var ids = staleDevices.Select(x => x.Id).ToArray();
                var sessions = await dbContext.TvConnectionHistory
                    .Where(x => ids.Contains(x.TvDeviceId) && x.DisconnectedAtUtc == null)
                    .ToListAsync(stoppingToken);

                foreach (var device in staleDevices)
                    device.MarkOffline();
                foreach (var session in sessions)
                    session.Close(session.LastHeartbeatAtUtc);

                await dbContext.SaveChangesAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested) { }
            catch (Exception exception)
            {
                logger.LogError(exception, "Falha ao atualizar o status das TVs.");
            }
        }
    }
}
