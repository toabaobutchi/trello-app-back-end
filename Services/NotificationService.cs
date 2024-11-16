using backend_apis.Data;
using backend_apis.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace backend_apis.Services
{
    public class NotificationService
    {
        private readonly IHubContext<NotificationHub> _hub;
        private readonly ProjectManagerDbContext _db;

        public NotificationService(IHubContext<NotificationHub> hub, ProjectManagerDbContext db)
        {
            _hub = hub;
            _db = db;
        }
        public async Task SendProjectNotification(Models.Notification notification, string? senderId = null)
        {
            try
            {
                var usersInProject = _db.HubConnections
                .Join(_db.Assignments,
                    hc => hc.UserId,
                    a => a.UserId,
                    (hc, a) => (new { Connection = hc, a.ProjectId })).Where(o => o.ProjectId == notification.ProjectId);
                if (usersInProject != null)
                {
                    foreach (var user in usersInProject)
                    {
                        if (user.Connection.UserId != senderId || senderId == null)
                        {
                            await _hub.Clients.Client(user?.Connection.ConnectionId).SendAsync("ReceiveProjectNotification", notification);
                        }
                    }
                }
            }
            catch
            {
                return;
            }
        }
        public async Task SendAssigneesNotification(IEnumerable<string> assignmentIds, Models.Notification notification)
        {
            try
            {
                
            }
            catch
            {
                return;
            }
        }
    }
}