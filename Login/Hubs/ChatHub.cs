using Login.models.setting;
using Microsoft.AspNetCore.SignalR;

namespace Login.Hubs
{
    public class ChatHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            await Clients.All.SendAsync("ReceviceMessage", $"{Context.ConnectionId} has joined");
        }

/*        public async Task JoinSpecificChatGroup(UserConnection connection)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, connection.ChatRoom);
            await Clients.Groups(connection.ChatRoom).SendAsync("JoinSpecificChatGroup", "admin", $"{connection.UserName} has joined");
        }*/

    }
}
