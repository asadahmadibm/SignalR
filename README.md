# SignalR
    dotnet new razor --auth Individual
    dotnet add package Microsoft.AspNetCore.SignalR --version 1.0.4

## Add a SignalR Hub

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
    
    
    
    Microsoft.AspNetCore.Authentication.JwtBearer 


