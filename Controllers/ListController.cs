using backend_apis.ApiModels.RequestModels;
using backend_apis.ApiModels.ResponseModels;
using backend_apis.Filters;
using backend_apis.Hubs;
using backend_apis.Repositories;
using backend_apis.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend_apis.Controllers
{
    [ApiController]
    [Route("/api")]
    [Authorize]
    [ProjectAuthorize]
    public class ListController : ProjectControllerBase
    {
        private readonly IListRepository _listRepo;

        public ListController(IListRepository listRepo)
        {
            _listRepo = listRepo;
        }
        [HttpPost("lists")]
        [ValidateModel]
        [ProjectRole(ContextResponse.Admin, ContextResponse.Owner)]
        public async Task<IActionResult> Create([FromBody] CreateListModel model)
        {
            var (list, listOrder) = await _listRepo.CreateListAsync(model, ProjectCookie);
            if (list == null)
            {
                return ResponseHelper.CannotHandle(ResponseMessage.F_CREATE);
            }
            return ResponseHelper.Ok(CreateListResponse.Create(list, listOrder), ResponseMessage.S_CREATE);
        }

        [HttpPut("lists/change-order")]
        [ValidateModel]
        [ProjectRole(ContextResponse.Admin, ContextResponse.Owner)]
        public async Task<IActionResult> Update([FromBody] UpdateListOrder model)
        {
            var newListOrder = await _listRepo.ChangeListOrderAsync(model, ProjectCookie);
            if (newListOrder == null)
            {
                return ResponseHelper.CannotHandle(ResponseMessage.F_UPDATE);
            }
            return ResponseHelper.Ok(newListOrder, ResponseMessage.S_UPDATE);
        }

        [HttpPut("lists/{lid}")]
        [ValidateModel]
        [ProjectRole(ContextResponse.Admin, ContextResponse.Owner)]
        public async Task<IActionResult> Update([FromRoute] string lid, [FromBody] UpdateListModel model)
        {
            var list = await _listRepo.UpdateAsync(model, lid, ProjectCookie);

            if (list == null)
            {
                return ResponseHelper.NotFound();
            }
            var responseList = UpdatedListResponse.Create(list);
            return ResponseHelper.Ok(responseList, ResponseMessage.S_UPDATE);
        }

        [HttpDelete("lists/{lid}")]
        [ProjectRole(ContextResponse.Admin, ContextResponse.Owner)]
        public async Task<IActionResult> Delete([FromRoute] string lid)
        {
            var deletedList = await _listRepo.DeleteAsync(lid, ProjectCookie);
            if (deletedList == null)
            {
                return ResponseHelper.CannotHandle(ResponseMessage.F_DELETE);
            }
            return ResponseHelper.Ok(DeletedListResponse.Create(deletedList), ResponseMessage.S_DELETE);
        }
    }
}