using Microsoft.AspNetCore.SignalR;
using Quartz;
using SignalRChat.Hubs;

namespace SignalRChat.QuartzServices
{
    [DisallowConcurrentExecution]
    public class NonConconcurrentJob : IJob
    {
        private readonly ILogger<NonConconcurrentJob> _logger;
        private static int _counter = 0;
        private readonly IHubContext<Chat> _hubContext;

        public NonConconcurrentJob(ILogger<NonConconcurrentJob> logger,
               IHubContext<Chat> hubContext)
        {
            _logger = logger;
            _hubContext = hubContext;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var count = _counter++;

            var beginMessage = $"NonConconcurrentJob Job BEGIN {count} {DateTime.UtcNow}";
            await _hubContext.Clients.All.SendAsync("NonConcurrentJobs", beginMessage);
            _logger.LogInformation("{beginMessage}", beginMessage);

            Thread.Sleep(5000);

            var endMessage = $"NonConconcurrentJob Job END {count} {DateTime.UtcNow}";
            await _hubContext.Clients.All.SendAsync("NonConcurrentJobs", endMessage);
            _logger.LogInformation("{endMessage}", endMessage);
        }
    }
}
