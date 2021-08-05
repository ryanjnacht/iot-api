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
            _logger.LogInformation("Worker started at: {Time}", DateTime.Now);

            while (!stoppingToken.IsCancellationRequested)
            {
                var now = new TimeSpan(DateTime.Now.TimeOfDay.Hours, DateTime.Now.TimeOfDay.Minutes,
                    DateTime.Now.TimeOfDay.Seconds);

                Parallel.ForEach(ScheduleRepository.Get().FindAll(x => x.ShouldRun(now)), scheduleObj =>
                {
                    _logger.LogInformation("Schedule {ScheduleId} running at: {Time}", scheduleObj.Id, DateTime.Now);

                    Parallel.ForEach(scheduleObj.Devices, schedulerDeviceObj =>
                    {
                        var retry = 1;
                        while (true)
                            try
                            {
                                var deviceObj = DeviceRepository.Get(schedulerDeviceObj.DeviceId);

                                if (schedulerDeviceObj.Action == null)
                                    continue;

                                switch (schedulerDeviceObj.Action.ToLower())
                                {
                                    case "on":
                                    case "turnon":
                                        deviceObj.TurnOn();
                                        break;
                                    case "off":
                                    case "turnoff":
                                        deviceObj.TurnOff();
                                        break;
                                    case "toggle":
                                        deviceObj.Toggle();
                                        break;
                                }

                                return;
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(
                                    "scheduleId: {ScheduleId}, deviceId: {DeviceId}, attempt: {Retry}/{MaxRetries}, error: {Exception}",
                                    scheduleObj.Id, schedulerDeviceObj.DeviceId, retry.ToString(), 
                                    Configuration.Configuration.DeviceRetries, ex.Message);

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