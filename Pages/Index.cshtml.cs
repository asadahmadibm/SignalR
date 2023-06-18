using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using SignalRChat.Hubs;

namespace SignalRChat.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(ILogger<IndexModel> logger, 
        IHubContext<Chat> hubContext)
    {
        _logger = logger;
    }

    public void OnGet()
    {
    }

   
}
