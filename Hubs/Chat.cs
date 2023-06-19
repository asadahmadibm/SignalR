using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace SignalRChat.Hubs
{
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    //[Authorize]
    public class Chat : Hub
    {

        public override Task OnConnectedAsync()
        {
            //var user = GetUser();
            //_usersOnline.Add(user);
            //Clients.All.listUpdated(_usersOnline);
            return base.OnConnectedAsync();
        }
        
         

        
        public async Task SendMessage(string message)
        {
            await Clients.All.SendAsync("newMessage", Context.User?.Identity?.Name ?? "Unknown", message);
        }


        public Task SendConcurrentJobsMessage(string message)
        {
            return Clients.All.SendAsync("ConcurrentJobs", message);
        }

        public Task SendNonConcurrentJobsMessage(string message)
        {
            return Clients.All.SendAsync("NonConcurrentJobs", message);
        }


    }
}
