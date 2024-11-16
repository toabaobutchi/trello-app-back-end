using backend_apis.ApiModels.RequestModels;
using backend_apis.ApiModels.ResponseModels;
using backend_apis.Data;
using backend_apis.Filters;
using backend_apis.Repositories;
using backend_apis.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend_apis.Controllers
{
    [ApiController]
    [Route("api/comments")]
    [Authorize]
    [ProjectAuthorize]
    public class CommentController : AppControllerBase
    {
        private readonly ProjectManagerDbContext _db;
        private readonly ICommentRepository _commentRepo;

        public CommentController(ProjectManagerDbContext db, ICommentRepository commentRepo)
        {
            _db = db;
            _commentRepo = commentRepo;
        }
        [HttpGet("in-task/{tid}")]
        public async Task<IActionResult> GetComments([FromRoute] string tid)
        {
            var parseResult = Guid.TryParse(tid, out var validId);
            if (!parseResult)
            {
                return ResponseHelper.BadRequest(null, ResponseMessage.INVALID_DATA);
            }
            var comments = await _db.Comments.Where(c => c.TaskId == validId).ToListAsync();
            var res = comments.Select(c => CommentResponse.Create(c));

            return ResponseHelper.Ok(res, ResponseMessage.S_FETCH);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCommentModel model)
        {
            var comment = model.ToComment();
            await _db.Comments.AddAsync(comment);
            await _db.SaveChangesAsync();

            return ResponseHelper.Ok(CommentResponse.Create(comment), ResponseMessage.S_CREATE);
        }

        [HttpGet("in-project/{pid}")]
        public async Task<IActionResult> GetProjectComments([FromRoute] string pid)
        {
            var response = await _commentRepo.GetProjectCommentsAsync(pid);
            if (response == null)
            {
                return ResponseHelper.NotFound();
            }
            return ResponseHelper.Ok(response, ResponseMessage.S_FETCH);
        }

        [HttpPost("in-project")]
        public async Task<IActionResult> CreateProjectComment([FromBody] CreateProjectCommentModel model)
        {
            var comment = await _commentRepo.CreateProjectCommentAsync(ProjectCookie, model);
            if (comment == null)
            {
                return ResponseHelper.CannotHandle();
            }
            return ResponseHelper.Ok(comment, ResponseMessage.S_CREATE);
        }
    }
}