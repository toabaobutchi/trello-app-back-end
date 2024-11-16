using backend_apis.ApiModels.ResponseForBoardDisplay;
using backend_apis.ApiModels.ResponseModels;
using backend_apis.Data;
using backend_apis.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace backend_apis.Hubs
{
    [Authorize(AuthenticationSchemes = ProjectAuthentication.AuthenticationScheme, Policy = ProjectAuthentication.HubPolicy)]
    public class ProjectHub : Hub
    {
        private readonly ProjectManagerDbContext _db;

        public ProjectHub(ProjectManagerDbContext db)
        {
            _db = db;
        }
        public override async Task OnConnectedAsync()
        {
            var user = Context.User;
            var projectId = user?.FindFirst(ProjectClaimType.ProjectId)?.Value;
            var assignmentId = user?.FindFirst(ProjectClaimType.AssignmentId)?.Value;
            await Groups.AddToGroupAsync(Context.ConnectionId, projectId);
            if (string.IsNullOrEmpty(projectId) || string.IsNullOrEmpty(assignmentId))
            {
                Context.Abort();
                return;
            }
            await SubscribeUser(projectId, assignmentId);
            await base.OnConnectedAsync();
        }
        public async Task SendGetOnlineMembers()
        {
            var (projectId, assignmentId, _) = GetProjectInfo();
            if (string.IsNullOrEmpty(projectId) || string.IsNullOrEmpty(assignmentId))
            {
                Context.Abort();
                return;
            }
            var connections = await _db.ProjectHubConnections.Where(c => c.ProjectId == projectId).Select(c => c.Id).ToListAsync();
            await Clients.Group(projectId).SendAsync("ReceiveOnlineMembers", connections);
        }
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            // Xử lý khi client ngắt kết nối ở đây
            var user = Context.User;
            var projectId = user?.FindFirst(ProjectClaimType.ProjectId)?.Value;
            var assignmentId = user?.FindFirst(ProjectClaimType.AssignmentId)?.Value;
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, projectId);
            if (string.IsNullOrEmpty(projectId) || string.IsNullOrEmpty(assignmentId))
            {
                Context.Abort();
                return;
            }
            await UnsubscribeUser(assignmentId);
            var connections = await _db.ProjectHubConnections.Where(c => c.ProjectId == projectId).Select(c => c.Id).ToListAsync();
            await Clients.OthersInGroup(projectId).SendAsync("ReceiveOnlineMembers", connections);
            await base.OnDisconnectedAsync(exception);
        }
        private async Task UnsubscribeUser(string assignmentId)
        {
            try
            {
                var connection = await _db.ProjectHubConnections.FindAsync(assignmentId);
                if (connection != null)
                {
                    _db.ProjectHubConnections.Remove(connection);
                    await _db.SaveChangesAsync();
                }
            }
            catch
            {
                return;
            }
        }
        private async Task SubscribeUser(string projectId, string assignmentId)
        {
            try
            {
                var hubConnection = await _db.ProjectHubConnections.FindAsync(assignmentId);
                if (hubConnection == null)
                {
                    hubConnection = new Models.ProjectHubConnection
                    {
                        Id = assignmentId,
                        ConnectionId = Context.ConnectionId,
                        ProjectId = projectId
                    };
                    await _db.ProjectHubConnections.AddAsync(hubConnection);
                }
                else
                {
                    hubConnection.ConnectionId = Context.ConnectionId;
                }
                await _db.SaveChangesAsync();
            }
            catch
            {
                return;
            }
        }
        private (string?, string?, Models.EPermission?) GetProjectInfo()
        {
            var user = Context.User;
            var projectId = user?.FindFirst(ProjectClaimType.ProjectId)?.Value;
            var assignmentId = user?.FindFirst(ProjectClaimType.AssignmentId)?.Value;
            var role = user?.FindFirst(ProjectClaimType.ProjectPermission)?.Value;
            var parseResult = Enum.TryParse<Models.EPermission>(role, true, out var permission);
            return (projectId, assignmentId, parseResult ? permission : null);
        }
        public async Task SendStartDragList(string listId)
        {
            var (projectId, assignmentId, permission) = GetProjectInfo();
            if (permission != null)
            {
                if (permission == Models.EPermission.Admin || permission == Models.EPermission.Owner)
                {
                    await Clients.OthersInGroup(projectId).SendAsync(
                        "ReceiveStartDragList",
                        assignmentId,
                        listId
                    );
                }
            }
        }
        public async Task SendStartDragTask(string listId, string taskId)
        {
            var (projectId, assignmentId, _) = GetProjectInfo();
            await Clients.OthersInGroup(projectId).SendAsync(
                "ReceiveStartDragTask",
                assignmentId,
                listId, taskId
            );
        }
        public async Task SendEndDragList(string updatedListOrder)
        {
            var (projectId, assignmentId, _) = GetProjectInfo();
            await Clients.OthersInGroup(projectId).SendAsync(
                "ReceiveEndDragList",
                assignmentId,
                updatedListOrder
            );
        }
        public async Task SendEndDragTask(ChangeTaskOrderResponse res, object dragResult)
        {
            var (projectId, assignmentId, _) = GetProjectInfo();
            await Clients.OthersInGroup(projectId).SendAsync(
                "ReceiveEndDragTask",
                assignmentId,
                res,
                dragResult
            );
        }
        public async Task SendStartUpdateTaskInfo(string taskId)
        {
            var (projectId, assignmentId, _) = GetProjectInfo();
            await Clients.OthersInGroup(projectId).SendAsync("ReceiveStartUpdateTaskInfo", assignmentId, taskId);
        }
        public async Task SendCancelUpdateTaskInfo(string taskId)
        {
            var (projectId, assignmentId, _) = GetProjectInfo();
            await Clients.OthersInGroup(projectId).SendAsync("ReceiveCancelUpdateTaskInfo", assignmentId, taskId);
        }
        public async Task SendUpdateTaskInfo(UpdatedTaskResponse data)
        {
            var (projectId, assignmentId, _) = GetProjectInfo();
            await Clients.OthersInGroup(projectId).SendAsync("ReceiveUpdateTaskInfo", assignmentId, data);
        }
        public async Task SendCheckSubtask(string taskId, int subtaskId, bool status)
        {
            var (projectId, assignmentId, _) = GetProjectInfo();
            await Clients.OthersInGroup(projectId).SendAsync("ReceiveCheckSubtask", assignmentId, taskId, subtaskId, status);
        }
        public async Task SendChangeSubtaskName(string taskId, int subtaskId, string name)
        {
            var (projectId, assignmentId, _) = GetProjectInfo();
            await Clients.OthersInGroup(projectId).SendAsync("ReceiveChangeSubtaskName", assignmentId, taskId, subtaskId, name);
        }
        public async Task SendAddingSubtasks(string taskId)
        {
            var (projectId, assignmentId, _) = GetProjectInfo();
            await Clients.OthersInGroup(projectId).SendAsync("ReceiveAddingSubtasks", assignmentId, taskId);
        }
        public async Task SendFinishAddSubtasks(string taskId)
        {
            var (projectId, assignmentId, _) = GetProjectInfo();
            await Clients.OthersInGroup(projectId).SendAsync("ReceiveFinishAddSubtasks", assignmentId, taskId);
        }
        public async Task SendAddSubtaskResult(string taskId, IEnumerable<SubtaskForBoard> subtasks)
        {
            var (projectId, assignmentId, _) = GetProjectInfo();
            await Clients.OthersInGroup(projectId).SendAsync("ReceiveAddSubtaskResult", assignmentId, taskId, subtasks);
        }
        public async Task SendDeleteSubtask(string taskId, int subtaskId)
        {
            var (projectId, assignmentId, _) = GetProjectInfo();
            await Clients.OthersInGroup(projectId).SendAsync("ReceiveDeleteSubtask", assignmentId, taskId, subtaskId);
        }
        public async Task SendAddNewTask(CreateTaskResponse data)
        {
            var (projectId, assignmentId, _) = GetProjectInfo();
            await Clients.OthersInGroup(projectId).SendAsync("ReceiveAddNewTask", assignmentId, data);
        }
        public async Task SendComment(CommentResponse commentResponse)
        {
            var (projectId, _, _) = GetProjectInfo();
            await Clients.OthersInGroup(projectId).SendAsync("RecieveComment", commentResponse);
        }
        public async Task SendAddNewList(CreateListResponse data)
        {
            var (projectId, assignmentId, _) = GetProjectInfo();
            await Clients.OthersInGroup(projectId).SendAsync("ReceiveAddNewList", assignmentId, data);
        }
        public async Task SendDeleteList(DeletedListResponse data)
        {
            var (projectId, assignmentId, _) = GetProjectInfo();
            await Clients.OthersInGroup(projectId).SendAsync("ReceiveDeleteList", assignmentId, data);
        }
        public async Task SendMarkTask(MarkedTaskResponse data)
        {
            var (projectId, assignmentId, _) = GetProjectInfo();
            await Clients.OthersInGroup(projectId).SendAsync("ReceiveMarkTask", assignmentId, data);
        }
        public async Task SendAssignMemberToTask(AssignByTaskResponse data)
        {
            var (projectId, assignmentId, _) = GetProjectInfo();
            await Clients.OthersInGroup(projectId).SendAsync("ReceiveAssignMemberToTask", assignmentId, data);
        }
        public async Task SendJoinSubtask(JoinSubtaskResponse data)
        {
            var (projectId, assignmentId, _) = GetProjectInfo();
            await Clients.OthersInGroup(projectId).SendAsync("ReceiveJoinSubtask", assignmentId, data);
        }
        public async Task SendAssignSubtask(AssignSubtaskResponse data)
        {
            var (projectId, assignmentId, _) = GetProjectInfo();
            await Clients.OthersInGroup(projectId).SendAsync("ReceiveAssignSubtask", assignmentId, data);
        }
        public async Task SendUnassignSubtask(UnassignSubtaskResponse data)
        {
            var (projectId, assignmentId, _) = GetProjectInfo();
            await Clients.OthersInGroup(projectId).SendAsync("ReceiveUnassignSubtask", assignmentId, data);
        }
        public async Task SendDuplicateTasks(IEnumerable<TaskResponseForBoardDisplay> duplicatedTasks)
        {
            var (projectId, assignmentId, _) = GetProjectInfo();
            await Clients.OthersInGroup(projectId).SendAsync("ReceiveDuplicateTasks", assignmentId, duplicatedTasks);
        }
        public async Task SendJoinTask(JoinTaskResponse data)
        {
            var (projectId, assignmentId, _) = GetProjectInfo();
            await Clients.OthersInGroup(projectId).SendAsync("ReceiveJoinTask", assignmentId, data);
        }
        public async Task SendUnassignTaskAssignment(DeletedTaskAssignmentResponse data)
        {
            var (projectId, assignmentId, _) = GetProjectInfo();
            await Clients.OthersInGroup(projectId).SendAsync("ReceiveUnassignTaskAssignment", assignmentId, data);
        }
        public async Task SendProjectComment(ProjectCommentResponse data)
        {
            var (projectId, _, _) = GetProjectInfo();
            await Clients.OthersInGroup(projectId).SendAsync("ReceiveProjectComment", data);
        }
        public async Task SendAddTaskDependencies(string taskId, IEnumerable<RelatedTaskResponse> relatedTasks)
        {
            var (projectId, assignmentId, _) = GetProjectInfo();
            await Clients.OthersInGroup(projectId).SendAsync("ReceiveAddTaskDependencies", assignmentId, taskId, relatedTasks);
        }
        public async Task SendAddChildrenTasks(string taskId, IEnumerable<RelatedTaskResponse> relatedTasks)
        {
            var (projectId, assignmentId, _) = GetProjectInfo();
            await Clients.OthersInGroup(projectId).SendAsync("ReceiveAddChildrenTasks", assignmentId, taskId, relatedTasks);
        }
        public async Task SendRemoveTaskDependency(DeletedRelationshipResponse data)
        {
            var (projectId, assignmentId, _) = GetProjectInfo();
            await Clients.OthersInGroup(projectId).SendAsync("ReceiveRemoveTaskDependency", assignmentId, data.TaskId, data);
        }
        public async Task SendRemoveChildrenTask(DeletedRelationshipResponse data)
        {
            var (projectId, assignmentId, _) = GetProjectInfo();
            await Clients.OthersInGroup(projectId).SendAsync("ReceiveRemoveChildrenTask", assignmentId, data.TaskId, data);
        }
        public async Task SendDeleteTask(DeletedTaskResponse data, bool moveToTrash = false)
        {
            var (projectId, assignmentId, _) = GetProjectInfo();
            await Clients.OthersInGroup(projectId).SendAsync("ReceiveDeleteTask", assignmentId, data.Id, data, moveToTrash);
        }
        public async Task SendRemoveAssignment(DeletedAssignmentResponse data)
        {
            var (projectId, assignmentId, _) = GetProjectInfo();
            await Clients.OthersInGroup(projectId).SendAsync("ReceiveRemoveAssignment", assignmentId, data);
        }
        public async Task SendUpdateWIP(UpdatedListResponse data)
        {
            var (projectId, assignmentId, _) = GetProjectInfo();
            await Clients.OthersInGroup(projectId).SendAsync("ReceiveUpdateWIP", assignmentId, data);
        }
    }
}