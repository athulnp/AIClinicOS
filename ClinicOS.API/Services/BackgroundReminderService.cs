using ClinicOS.Application.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ClinicOS.API.Services;

/// <summary>
/// Background service for sending reminders
/// </summary>
public class BackgroundReminderService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<BackgroundReminderService> _logger;
    private readonly TimeSpan _checkInterval = TimeSpan.FromHours(1); // Check every hour

    public BackgroundReminderService(
        IServiceProvider serviceProvider,
        ILogger<BackgroundReminderService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Background Reminder Service is starting");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var reminderService = scope.ServiceProvider.GetRequiredService<IReminderService>();

                _logger.LogInformation("Checking for reminders to send at: {time}", DateTimeOffset.Now);

                // Send appointment reminders
                await reminderService.SendAppointmentRemindersAsync();

                // Send follow-up reminders
                await reminderService.SendFollowUpRemindersAsync();

                _logger.LogInformation("Reminder check completed at: {time}", DateTimeOffset.Now);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while processing reminders");
            }

            await Task.Delay(_checkInterval, stoppingToken);
        }

        _logger.LogInformation("Background Reminder Service is stopping");
    }
}
