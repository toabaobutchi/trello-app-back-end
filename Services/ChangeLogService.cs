using backend_apis.ApiModels.ResponseModels;
using backend_apis.Hubs;
using backend_apis.Models;
using Microsoft.AspNetCore.SignalR;

namespace backend_apis.Services
{
    public class ChangeLogService
    {
        private readonly IHubContext<ProjectHub> _hub;

        public ChangeLogService(IHubContext<ProjectHub> hub)
        {
            _hub = hub;
        }
        public System.Threading.Tasks.Task SendChangeLogAsync(string projectId, ChangeLog changeLog)
        {
            return System.Threading.Tasks.Task.Run(async () =>
            {
                await _hub.Clients
                        .Group(projectId)
                        .SendAsync("ReceiveChangeLog", ChangeLogResponse.Create(changeLog));
            });
        }
    }
}