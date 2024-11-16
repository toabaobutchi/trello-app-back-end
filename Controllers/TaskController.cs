using backend_apis.ApiModels.RequestModels;
using backend_apis.ApiModels.ResponseModels;
using backend_apis.Filters;
using backend_apis.Repositories;
using backend_apis.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend_apis.Controllers
{
    [ApiController]
    [Route("api/tasks")]
    [Authorize]
    [ProjectAuthorize]
    public class TaskController : ProjectControllerBase
    {
        private readonly ITaskRepository _taskRepo;

        public TaskController(ITaskRepository taskRepo)
        {
            _taskRepo = taskRepo;
        }

        [HttpGet("{tid}/v/{viewMode}")]
        public async Task<IActionResult> GetTaskDetailForBoard([FromRoute] string tid, [FromRoute] string viewMode)
        {
            var task = await _taskRepo.GetTaskDetailAsync(tid);
            if (task == null)
            {
                return ResponseHelper.CannotHandle(ResponseMessage.F_FETCH);
            }
            return ResponseHelper.Ok(task, ResponseMessage.S_FETCH);
        }

        [HttpPost("tasks")]
        [ValidateModel]
        [ProjectRole(ContextResponse.Admin, ContextResponse.Owner)]
        public async Task<ActionResult> Create([FromBody] CreateTaskModel model)
        {
            var (task, taskOrder) = await _taskRepo.CreateTaskAsync(model, ProjectCookie);
            if (task == null)
            {
                return ResponseHelper.CannotHandle(ResponseMessage.F_CREATE);
            }
            return ResponseHelper.Ok(CreateTaskResponse.Create(task, taskOrder), ResponseMessage.S_CREATE);
        }

        [HttpPut("{tid}/change-order")]
        [ValidateModel]
        public async Task<ActionResult> ChangeOrder([FromRoute] string tid, [FromBody] ChangeTaskOrderModel model)
        {
            var result = await _taskRepo.ChangeTaskOrderAsync(model, tid, ProjectCookie);
            if (!result)
            {
                return ResponseHelper.CannotHandle(ResponseMessage.F_UPDATE);
            }
            return ResponseHelper.Ok(ChangeTaskOrderResponse.Create(model), ResponseMessage.S_UPDATE);
        }

        [HttpPut("{tid}")]
        [ProjectRole(ContextResponse.Admin, ContextResponse.Owner)]
        [ValidateModel]
        public async Task<IActionResult> Update([FromRoute] string tid, [FromBody] UpdateTaskModel model)
        {
            var task = await _taskRepo.UpdateTaskAsync(tid, model, ProjectCookie);
            if (task == null)
            {
                return ResponseHelper.NotFound();
            }
            return ResponseHelper.Ok(UpdatedTaskResponse.Create(task), ResponseMessage.S_UPDATE);
        }

        [HttpPost("{tid}/duplicate")]
        [ProjectRole(ContextResponse.Admin, ContextResponse.Owner)]
        public async Task<IActionResult> Duplicate([FromRoute] string tid, [FromBody] DuplicateTaskModel model)
        {
            var duplicatedTasks = await _taskRepo.DuplicateTaskAsync(model, tid, ProjectCookie);
            if (duplicatedTasks == null)
            {
                return ResponseHelper.CannotHandle();
            }
            return ResponseHelper.Ok(duplicatedTasks, ResponseMessage.S_CREATE);
        }

        [HttpDelete("{tid}")]
        [ProjectRole(ContextResponse.Admin, ContextResponse.Owner)]
        public async Task<IActionResult> Delete([FromRoute] string tid)
        {
            var deletedTask = await _taskRepo.DeleteTaskAsync(tid, ProjectCookie);
            if (deletedTask == null)
            {
                return ResponseHelper.CannotHandle(ResponseMessage.F_DELETE);
            }
            return ResponseHelper.Ok(new DeletedTaskResponse() { Id = deletedTask.Id.ToString(), ListId = deletedTask?.ListId ?? "" }, ResponseMessage.S_DELETE);
        }

        [HttpDelete("/api/v2/tasks/{tid}")]
        [ProjectRole(ContextResponse.Admin, ContextResponse.Owner)]
        public async Task<IActionResult> Delete_v2([FromRoute] string tid)
        {
            var deletedTask = await _taskRepo.DeleteTaskAsync_v2(tid, ProjectCookie);
            if (deletedTask == null)
            {
                return ResponseHelper.CannotHandle(ResponseMessage.F_DELETE);
            }
            return ResponseHelper.Ok(new DeletedTaskResponse() { Id = deletedTask.Id.ToString(), ListId = deletedTask?.ListId ?? "" }, ResponseMessage.S_DELETE);
        }

        [HttpPut("{tid}/mark")]
        public async Task<IActionResult> Mark([FromRoute] string tid, [FromBody] MarkTaskModel model)
        {
            var markedTask = await _taskRepo.MarkTaskAsync(tid, ProjectCookie, model);
            if (markedTask == null)
            {
                return ResponseHelper.CannotHandle();
            }
            var responseTask = MarkedTaskResponse.Create(markedTask);

            return ResponseHelper.Ok(responseTask, ResponseMessage.S_UPDATE);
        }

        [HttpPost("{tid}/join")]
        public async Task<IActionResult> Join([FromRoute] string tid)
        {
            var taskAssignment = await _taskRepo.JoinTaskAsync(tid, ProjectCookie);
            if (taskAssignment == null)
            {
                return ResponseHelper.NotFound();
            }
            var res = JoinTaskResponse.Create(taskAssignment);

            return ResponseHelper.Ok(res, ResponseMessage.S_UPDATE);
        }

        [HttpDelete("{tid}/move-to-trash")]
        [ProjectRole(ContextResponse.Admin, ContextResponse.Owner)]
        public async Task<IActionResult> MoveToTrash([FromRoute] string tid)
        {
            var deletedTask = await _taskRepo.MoveTaskToTrashAsync(tid, ProjectCookie);
            if (deletedTask == null)
            {
                return ResponseHelper.CannotHandle();
            }
            return ResponseHelper.Ok(new DeletedTaskResponse() { Id = deletedTask.Id.ToString(), ListId = deletedTask?.ListId ?? "" }, ResponseMessage.S_DELETE);
        }

        [HttpDelete("/api/v2/tasks/{tid}/move-to-trash")]
        [ProjectRole(ContextResponse.Admin, ContextResponse.Owner)]
        public async Task<IActionResult> MoveToTrash_v2([FromRoute] string tid)
        {
            var deletedTask = await _taskRepo.MoveTaskToTrashAsync_v2(tid, ProjectCookie);
            if (deletedTask == null)
            {
                return ResponseHelper.CannotHandle();
            }
            return ResponseHelper.Ok(new DeletedTaskResponse() { Id = deletedTask.Id.ToString(), ListId = deletedTask?.ListId ?? "" }, ResponseMessage.S_DELETE);
        }

        [HttpPut("{tid}/reset")]
        [ProjectRole(ContextResponse.Admin, ContextResponse.Owner)]
        public async Task<IActionResult> ResetTaskInfo([FromRoute] string tid, ResetTaskModel model)
        {
            var resetTask = await _taskRepo.ResetTaskAsync(tid, ProjectCookie, model);
            if (resetTask == null)
            {
                return ResponseHelper.CannotHandle(ResponseMessage.F_UPDATE);
            }
            return ResponseHelper.Ok(resetTask, ResponseMessage.S_UPDATE);
        }

        [HttpPost("{tid}/add-dependencies")]
        [ProjectRole(ContextResponse.Admin, ContextResponse.Owner)]
        public async Task<IActionResult> AddTaskDependencies([FromRoute] string tid, [FromBody] AddTaskDependenciesModel model)
        {
            var taskDependenciesIds = await _taskRepo.AddDependenciesAsync(tid, ProjectCookie, model);
            if (taskDependenciesIds == null)
            {
                return ResponseHelper.CannotHandle(ResponseMessage.F_UPDATE);
            }
            return ResponseHelper.Ok(taskDependenciesIds, ResponseMessage.S_UPDATE);
        }

        [HttpGet("{tid}/dependencies")]
        public async Task<IActionResult> GetDependencies([FromRoute] string tid)
        {
            var results = await _taskRepo.GetDependenciesAsync(tid);
            if (results == null)
            {
                return ResponseHelper.NotFound();
            }
            return ResponseHelper.Ok(results, ResponseMessage.S_FETCH);
        }

        [HttpGet("{tid}/related-tasks")]
        public async Task<IActionResult> GetRelatedTasks([FromRoute] string tid)
        {
            var result = await _taskRepo.GetReferenceTasksAsync(tid);
            if (result == null)
            {
                return ResponseHelper.NotFound();
            }
            return ResponseHelper.Ok(result, ResponseMessage.S_FETCH);
        }

        [HttpPost("{tid}/add-children")]
        [ProjectRole(ContextResponse.Admin, ContextResponse.Owner)]
        public async Task<IActionResult> AddChildrenTask([FromRoute] string tid, [FromBody] AddChildrenTaskModel model)
        {
            var taskChildrenIds = await _taskRepo.AddChildrenAsync(tid, ProjectCookie, model);
            if (taskChildrenIds == null)
            {
                return ResponseHelper.CannotHandle(ResponseMessage.F_UPDATE);
            }
            return ResponseHelper.Ok(taskChildrenIds, ResponseMessage.S_UPDATE);
        }

        [HttpDelete("{tid}/{relationship}/{rid}")]
        [ProjectRole(ContextResponse.Admin, ContextResponse.Owner)]
        public async Task<IActionResult> DeleteTaskRelationship([FromRoute] string tid, [FromRoute] string relationship, [FromRoute] string rid)
        {
            var result = await _taskRepo.DeleteTaskRelationshipAsync(tid, ProjectCookie, relationship, rid);
            if (result == null)
            {
                return ResponseHelper.NotFound();
            }
            return ResponseHelper.Ok(result, ResponseMessage.S_DELETE);
        }

        [HttpPut("{tid}/restore")]
        [ProjectRole(ContextResponse.Admin, ContextResponse.Owner)]
        public async Task<IActionResult> Restore([FromRoute] string tid)
        {
            var result = await _taskRepo.RestoreAsync(tid, ProjectCookie);
            if (result == null)
            {
                return ResponseHelper.NotFound();
            }
            return ResponseHelper.Ok(result, ResponseMessage.S_UPDATE);
        }
    }
}