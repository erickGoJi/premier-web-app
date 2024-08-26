using Microsoft.AspNetCore.SignalR;
using biz.premier.Entities;
using Task = System.Threading.Tasks.Task;

namespace api.premier.Hubs
{
    public class MessageHub : Hub
    {
        public async System.Threading.Tasks.Task NewConversation(ChatConversationImmigrationRelocation msg)
        {
            await Clients.All.SendAsync("MessageReceived", msg);
        }

        public async System.Threading.Tasks.Task NewMessage(ChatImmigrationRelocation msg)
        {
            await Clients.All.SendAsync("MessageReceived", msg);
        }
    }
}
