using backend_apis.Data;
using backend_apis.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace backend_apis.Hubs
{
    [Authorize]
    public class NotificationHub : Hub
    {
        private readonly ProjectManagerDbContext _db;

        public NotificationHub(ProjectManagerDbContext db)
        {
            _db = db;
        }
        public async Task SubscribeNotification()
        {
            try
            {
                await Clients.Caller.SendAsync("RecievedSubscribeNitification");

                var userId = Context.User?.FindFirst(UserClaimType.UserId)?.Value ?? "";
                var userConnection = await _db.HubConnections.FindAsync(userId);
                if (userConnection == null)
                {
                    userConnection = new Models.HubConnection { UserId = userId, ConnectionId = Context.ConnectionId };
                    _db.HubConnections.Add(userConnection);
                }
                userConnection.ConnectionId = Context.ConnectionId; // thay doi connection id
                await _db.SaveChangesAsync();
            }
            catch
            {
                return;
            }
        }
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userConnection = _db.HubConnections.FirstOrDefault(c => c.ConnectionId == Context.ConnectionId);
            if (userConnection != null)
            {
                _db.HubConnections.Remove(userConnection);
                await _db.SaveChangesAsync();
            }
            await base.OnDisconnectedAsync(exception);
        }
    }
}