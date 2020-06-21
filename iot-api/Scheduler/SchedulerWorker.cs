using System;
using System.Threading;
using System.Threading.Tasks;
using iot_api.Repository;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace iot_api.Scheduler
{
    public class SchedulerWorker : BackgroundService
    {
        private readonly ILogger<SchedulerWorker> _logger;

        public SchedulerWorker(ILogger<SchedulerWorker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Worker started at: {time}", DateTime.Now);

            while (!stoppingToken.IsCancellationRequested)
            {
                var now = new TimeSpan(DateTime.Now.TimeOfDay.Hours, DateTime.Now.TimeOfDay.Minutes,
                    DateTime.Now.TimeOfDay.Seconds);

                Parallel.ForEach(ScheduleRepository.Get().FindAll(x => x.ShouldRun(now)), scheduleObj =>
                {
                    _logger.LogInformation($"Schedule {scheduleObj.Id} running at: {DateTime.Now}");

                    Parallel.ForEach(scheduleObj.Devices, schedulerDeviceObj =>
                    {
                        var retry = 1;
                        while (true)
                            try
                            {
                                var deviceObj = DeviceRepository.Get(schedulerDeviceObj.DeviceId);

                                if (schedulerDeviceObj.Action == null)
                                    continue;

                                if (schedulerDeviceObj.Action.ToLower() == "on" ||
                                    schedulerDeviceObj.Action.ToLower() == "turnon")
                                    deviceObj.TurnOn();

                                if (schedulerDeviceObj.Action.ToLower() == "off" ||
                                    schedulerDeviceObj.Action.ToLower() == "turnoff")
                                    deviceObj.TurnOff();

                                if (schedulerDeviceObj.Action.ToLower() == "toggle")
                                    deviceObj.Toggle();

                                return;
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(
                                    $"scheduleId: {scheduleObj.Id}, deviceId: {schedulerDeviceObj.DeviceId}, " +
                                    $"attempt: {retry}/{Configuration.Configuration.DeviceRetries}, error: {ex.Message}");

                                if (retry >= Configuration.Configuration.DeviceRetries) return;

                                Thread.Sleep(1000 * retry * 5);
                                retry++;
                            }
                    });
                });

                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}