using backend_apis.ApiModels.ResponseForBoardDisplay;
using backend_apis.ApiModels.ResponseModels;
using backend_apis.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace backend_apis.Hubs
{
    [Authorize(AuthenticationSchemes = ProjectAuthentication.AuthenticationScheme, Policy = ProjectAuthentication.HubPolicy)]
    public class DragHub : Hub
    {
        public async Task SendAddToDragGroup(string projectId, string userId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, projectId);
        }
        public async Task SendStartDragList(string projectId, string userId, string listId)
        {
            await Clients.OthersInGroup(projectId).SendAsync(
                "ReceiveStartDragList",
                TextUtils.CreateAssignmentId(projectId, userId),
                listId
            );
        }
        public async Task SendStartDragTask(string projectId, string userId, string listId, string taskId)
        {
            await Clients.OthersInGroup(projectId).SendAsync(
                "ReceiveStartDragTask",
                TextUtils.CreateAssignmentId(projectId, userId),
                listId, taskId
            );
        }
        public async Task SendEndDragList(string projectId, string userId, string updatedListOrder)
        {
            await Clients.OthersInGroup(projectId).SendAsync(
                "ReceiveEndDragList",
                TextUtils.CreateAssignmentId(projectId, userId),
                updatedListOrder
            );
        }
        public async Task SendEndDragTask(string projectId, string userId, ChangeTaskOrderResponse res, object dragResult)
        {
            await Clients.OthersInGroup(projectId).SendAsync(
                "ReceiveEndDragTask",
                TextUtils.CreateAssignmentId(projectId, userId),
                res,
                dragResult
            );
        }
        public async Task SendStartUpdateTaskInfo(string projectId, string userId, string taskId)
        {
            await Clients.OthersInGroup(projectId).SendAsync("ReceiveStartUpdateTaskInfo", TextUtils.CreateAssignmentId(projectId, userId), taskId);
        }

        public async Task SendCancelUpdateTaskInfo(string projectId, string userId, string taskId)
        {
            await Clients.OthersInGroup(projectId).SendAsync("ReceiveCancelUpdateTaskInfo", TextUtils.CreateAssignmentId(projectId, userId), taskId);
        }

        public async Task SendUpdateTaskInfo(string projectId, string userId, UpdatedTaskResponse data)
        {
            await Clients.OthersInGroup(projectId).SendAsync("ReceiveUpdateTaskInfo", TextUtils.CreateAssignmentId(projectId, userId), data);
        }

        public async Task SendCheckSubtask(string projectId, string userId, string taskId, int subtaskId, bool status)
        {
            await Clients.OthersInGroup(projectId).SendAsync("ReceiveCheckSubtask", TextUtils.CreateAssignmentId(projectId, userId), taskId, subtaskId, status);
        }
        public async Task SendChangeSubtaskName(string projectId, string userId, string taskId, string subtaskId, string name)
        {
            await Clients.OthersInGroup(projectId).SendAsync("ReceiveChangeSubtaskName", TextUtils.CreateAssignmentId(projectId, userId), taskId, subtaskId, name);
        }
        public async Task SendAddingSubtasks(string projectId, string userId, string taskId)
        {
            await Clients.OthersInGroup(projectId).SendAsync("ReceiveAddingSubtasks", TextUtils.CreateAssignmentId(projectId, userId), taskId);
        }
        public async Task SendFinishAddSubtasks(string projectId, string userId, string taskId)
        {
            await Clients.OthersInGroup(projectId).SendAsync("ReceiveFinishAddSubtasks", TextUtils.CreateAssignmentId(projectId, userId), taskId);
        }
        public async Task SendAddSubtaskResult(string projectId, string userId, string taskId, IEnumerable<SubtaskForBoard> subtasks)
        {
            await Clients.OthersInGroup(projectId).SendAsync("ReceiveAddSubtaskResult", TextUtils.CreateAssignmentId(projectId, userId), taskId, subtasks);
        }
        public async Task SendDeleteSubtask(string projectId, string userId, string taskId, int subtaskId)
        {
            await Clients.OthersInGroup(projectId).SendAsync("ReceiveDeleteSubtask", TextUtils.CreateAssignmentId(projectId, userId), taskId, subtaskId);
        }
        public async Task SendAddNewTask(string projectId, string userId, CreateTaskResponse data)
        {
            await Clients.OthersInGroup(projectId).SendAsync("ReceiveAddNewTask", TextUtils.CreateAssignmentId(projectId, userId), data);
        }
    }
}