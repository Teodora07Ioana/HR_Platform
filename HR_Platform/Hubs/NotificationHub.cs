using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

/*namespace HR_Model.Hubs
{
    public class NotificationHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            if (Context.User.IsInRole("Manager"))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, "Managers");
            }
            await base.OnConnectedAsync();
        }

        public async Task SendNotificationToUser(string userId, string message)
        {
            await Clients.User(userId).SendAsync("ReceiveNotification", message);
        }

        public async Task BroadcastNotification(string message)
        {
            await Clients.All.SendAsync("ReceiveNotification", message);
        }
    }
}*/

namespace HR_Platform.Hubs
{
    public class NotificationHub : Hub
    {
        /*  public async Task SendNotification(string userId, string message)
          {
              Console.WriteLine($"Trimit notificare către utilizatorul {userId}: {message}");
              await Clients.User(userId).SendAsync("ReceiveNotification", message);
          }*/
        public async Task SendTestNotification()
        {
            await Clients.All.SendAsync("ReceiveNotification", "Test notificare din server!");
        }


        public override async Task OnConnectedAsync()
        {
            var email = Context.User?.FindFirst(ClaimTypes.Email)?.Value;
            if (!string.IsNullOrEmpty(email))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, email);
            }
            await base.OnConnectedAsync();
        }
      

    }
}
