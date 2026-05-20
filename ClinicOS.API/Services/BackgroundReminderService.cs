using ClinicOS.Application.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ClinicOS.API.Services;

public class BackgroundReminderService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<BackgroundReminderService> _logger;
    private readonly TimeSpan _checkInterval = TimeSpan.FromHours(1);

    public BackgroundReminderService(IServiceProvider serviceProvider, ILogger<BackgroundReminderService> logger)
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
                var clinicRepository = scope.ServiceProvider.GetRequiredService<IClinicRepository>();
                var tenantContext = scope.ServiceProvider.GetRequiredService<ITenantContext>();
                var reminderService = scope.ServiceProvider.GetRequiredService<IReminderService>();

                var clinics = await clinicRepository.GetAllAsync(activeOnly: true);

                foreach (var clinic in clinics)
                {
                    tenantContext.SetClinic(clinic.Id, clinic.Code);
                    await reminderService.SendAppointmentRemindersAsync();
                    await reminderService.SendFollowUpRemindersAsync();
                }

                _logger.LogInformation("Reminder check completed for {Count} clinics at: {Time}",
                    clinics.Count(), DateTimeOffset.Now);
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
