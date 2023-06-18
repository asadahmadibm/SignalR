# SignalR & Quartz
    dotnet new razor --auth Individual
    dotnet add package Microsoft.AspNetCore.SignalR --version 1.0.4

## Add a SignalR Hub
    1- Add In Startup
    public void ConfigureServices (IServiceCollection services)
    {
        // ...
        services.AddSignalR();
    }
    
    public void Configure(IApplicationBuilder app, HostingEnvironment env)
    {
        // ...
        app.UseAuthentication();
        app.UseMvc();
        
        app.UseEndpoints (builder =>
        {
            builder.MapHub<Chat>("/chat");
        });
    }
    
    2- Add in Class
    
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.SignalR;
    namespace SignalRChat.Hubs
    {
        public class Chat : Hub
        {
            public async Task SendMessage(string message)
            {
                await Clients.All.SendAsync("newMessage", "anonymous", message);
            }
        }
    }
    and  in other class in cs
    private readonly IHubContext<Chat> _hubContext;

        public changeJob(ILogger<changeJob> logger,
            IHubContext<Chat> hubContext)
        {
            _logger = logger;
            _hubContext = hubContext;
        }
    
    _hubContext.Clients.All.SendAsync("ConcurrentJobs", "beginMessage");

    3- Add in JavaScript code 

    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/chat")
        .configureLogging(signalR.LogLevel.Information)
        .build();
    
    
    async function start() {
        try {
            await connection.start();
            console.log("SignalR Connected.");
        } catch (err) {
            console.log(err);
            setTimeout(start, 5000);
        }
    };
    
    connection.onclose(async () => {
        await start();
    });
    
    start();
    
    connection.on("ConcurrentJobs", function (message) {
        var li = document.createElement("li");
        document.getElementById("concurrentJobs").appendChild(li);
        li.textContent = `${message}`;
    });

   example :
    
    Pages/Index.cshtml
    
    
    <script src="https://unpkg.com/@@aspnet/signalr@@1.0.0-rc1-final/dist/browser/signalr.js"></script>
    <div class="signalr-demo">
        <form id="message-form">
            <input type="text" id="message-box"/>
        </form>
        <hr />
        <ul id="messages"></ul>
    </div>
    
    Listing 1: Build and start a HubConnection with JavaScript
    
    <script>
        const messageForm =  document.getElementById('message-form');
        const messageBox = document.getElementById('message-box');
        const messages = document.getElementById('messages');
        
        const connection = new signalR.HubConnectionBuilder()
            .withUrl("/chat")
            .configureLogging(signalR.LogLevel.Information)
            .build();
            
        connection.on('newMessage', (sender, messageText) => {
            console.log(`${sender}:${messageText}`);
            
            const newMessage = document.createElement('li');
            newMessage.appendChild(document.createTextNode(`${sender}:${messageText}`));
            messages.appendChild(newMessage);
        });
        
        connection.start()
            .then(() => console.log('connected!'))
            .catch(console.error);
            
        messageForm.addEventListener('submit', ev => {
            ev.preventDefault();
            const message = messageBox.value;
            connection.invoke('SendMessage', message);
            messageBox.value = '';
        });
    </script>

## Add Quartz 

1-add nuget Quartz
2-add nuget Quartz.Extensions.Hosting


3- builder.Services.AddQuartz(q =>
{
    q.UseMicrosoftDependencyInjectionJobFactory();

    var conconcurrentJobKey = new JobKey("ConconcurrentJob");
    q.AddJob<ConconcurrentJob>(opts => opts.WithIdentity(conconcurrentJobKey));
    q.AddTrigger(opts => opts
        .ForJob(conconcurrentJobKey)
        .WithIdentity("ConconcurrentJob-trigger")
        .WithSimpleSchedule(x => x
            .WithIntervalInSeconds(5)
            .RepeatForever()));

});
builder.Services.AddQuartzHostedService(
    q => q.WaitForJobsToComplete = true);
	
	
4- 	public class ConconcurrentJob : IJob
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

            Thread.Sleep(2000);

            var endMessage = $"Conconcurrent Job END {count} {DateTime.UtcNow}";
            await _hubContext.Clients.All.SendAsync("ConcurrentJobs", endMessage);
            _logger.LogInformation("{endMessage}", endMessage);
        }
    }
	
	    

    



