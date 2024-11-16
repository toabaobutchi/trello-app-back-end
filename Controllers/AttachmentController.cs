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
    [Route("api/attachments")]
    [Authorize]
    public class AttachmentController : ControllerBase
    {
        private readonly IAttachmentRepository _attachmentRepo;

        public AttachmentController(IAttachmentRepository attachmentRepo)
        {
            _attachmentRepo = attachmentRepo;
        }
        [HttpGet("in-task/{tid}")]
        [Authorize(AuthenticationSchemes = ProjectAuthentication.AuthenticationScheme, Policy = ProjectAuthentication.RequiredPolicy)]
        public async Task<IActionResult> GetAttachments([FromRoute] string tid)
        {
            var attachments = await _attachmentRepo.GetAttachmentsAsync(tid);
            if (attachments == null)
            {
                return ResponseHelper.NotFound();
            }
            return ResponseHelper.Ok(attachments.Select(a => AttachmentResponse.Create(a)), ResponseMessage.S_FETCH);
        }
        [HttpPost]
        [Authorize(AuthenticationSchemes = ProjectAuthentication.AuthenticationScheme, Policy = ProjectAuthentication.RequiredPolicy)]
        [ValidateModel]
        public async Task<IActionResult> Create([FromBody] CreateAttachmentModel model)
        {
            var attachment = await _attachmentRepo.CreateAttachmentAsync(model);
            if (attachment == null)
            {
                return ResponseHelper.CannotHandle();
            }
            return ResponseHelper.Ok(AttachmentResponse.Create(attachment), ResponseMessage.S_CREATE);
        }
    }
}