using Microsoft.AspNetCore.SignalR;
using Quartz;
using SignalRChat.Hubs;

namespace SignalRChat.QuartzServices
{
    public class ConconcurrentJob : IJob
    {
        private readonly ILogger<ConconcurrentJob> _logger;
        private static int _counter = 0;
        private readonly IHubContext<Chat> _hubContext;

        public ConconcurrentJob(ILogger<ConconcurrentJob> logger,
            IHubContext<Chat> hubContext)
        {
            _logger = logger;
            _hubContext = hubContext;
        }

        public async Task Execute(IJobExecutionContext context)
        {

            var count = _counter++;

            var beginMessage = $"Conconcurrent Job BEGIN {count} {DateTime.UtcNow}";
            await _hubContext.Clients.All.SendAsync("ConcurrentJobs", beginMessage);
            _logger.LogInformation("{beginMessage}", beginMessage);

            Thread.Sleep(5000);

            var endMessage = $"Conconcurrent Job END {count} {DateTime.UtcNow}";
            await _hubContext.Clients.All.SendAsync("ConcurrentJobs", endMessage);
            _logger.LogInformation("{endMessage}", endMessage);


        }
    }
}