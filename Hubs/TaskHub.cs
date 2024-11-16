using backend_apis.ApiModels.ResponseModels;
using backend_apis.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace backend_apis.Hubs
{
    [Authorize(AuthenticationSchemes = ProjectAuthentication.AuthenticationScheme, Policy = ProjectAuthentication.HubPolicy)]
    public class TaskHub : Hub
    {
        public async Task SubscribeTaskGroup(string taskId)
        {
            var assignmentId = Context.User?.FindFirst(ProjectClaimType.AssignmentId)?.Value;
            System.Console.WriteLine($"Subscribe: {assignmentId} - taskId: {taskId}");
            await Groups.AddToGroupAsync(Context.ConnectionId, taskId);
            await Clients.OthersInGroup(taskId).SendAsync("RecieveSubscriber", taskId, assignmentId);
        }
        public async Task SendComment(string taskId, CommentResponse commentResponse)
        {
            System.Console.WriteLine(JSON.Stringify(commentResponse));
            Console.WriteLine(Clients.Group(taskId).ToString());
            await Clients.Caller.SendAsync("RecieveComment", commentResponse);
        }
    }
}