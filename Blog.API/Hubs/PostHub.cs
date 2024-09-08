using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

public class PostHub : Hub
{
    public async Task SendPostUpdate(string message)
    {
        await Clients.All.SendAsync("ReceivePostUpdate", message);
    }
}
