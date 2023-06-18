using Microsoft.AspNetCore.SignalR;
using Quartz;
using SignalRChat.Hubs;

namespace SignalRChat.QuartzServices
{
    public class changeJob : IJob
    {
        private readonly ILogger<changeJob> _logger;
        private static int _counter = 0;
        private readonly IHubContext<Chat> _hubContext;

        public changeJob(ILogger<changeJob> logger,
            IHubContext<Chat> hubContext)
        {
            _logger = logger;
            _hubContext = hubContext;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            Random rnd = new Random();
            int divnumber = rnd.Next(1, 6);
            int amount = rnd.Next(1, 10000);

            await _hubContext.Clients.All.SendAsync("spanupdate", divnumber, amount.ToString());


        }
    }
}