using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TasksHandler.Entities;
using TasksHandler.Services;

namespace TasksHandler.Controllers
{
    [Route("api/files")]
    public class FilesController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly IFilesRepository filesRepository;
        private readonly IUsersService usersService;
        private readonly string container = "attachedfiles";

        public FilesController(ApplicationDbContext context,
            IFilesRepository filesRepository,
            IUsersService usersService)
        {
            this.context = context;
            this.filesRepository = filesRepository;
            this.usersService = usersService;
        }

        [HttpPost("{taskId:int}")]
        public async Task<ActionResult<IEnumerable<AttachedFile>>> Post(int taskId, [FromForm] IEnumerable<IFormFile> files )
        {
            var userId = usersService.getUserId();

            var task = await context.Tasks.FirstOrDefaultAsync(t => t.Id== taskId);

            if (task == null)
            {
                return NotFound();
            }

            var existAttachedFiles = await context.AttachedFiles.AnyAsync(a => a.TaskId == taskId);

            var majorOrder = 0;
            if (existAttachedFiles)
            {
                majorOrder = await context.AttachedFiles
                    .Where(a => a.TaskId == taskId).Select(a => a.Orden).MaxAsync();
            }

            var results = await filesRepository.Store(container, files);

            var attachedFiles = results.Select((result, index) => new AttachedFile
            {
                TaskId= taskId,
                CreationDate= DateTime.UtcNow,
                Url = result.URL,
                Tittle = result.Title,
                Orden = majorOrder + index + 1
            }).ToList();

            context.AddRange(attachedFiles);
            await context.SaveChangesAsync();

            return attachedFiles.ToList();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, [FromBody] string title)
        {
            var userId = usersService.getUserId();

            var attachedFile = await context.AttachedFiles.
                Include(a => a.Task).FirstOrDefaultAsync(a => a.Id == id);

            if (attachedFile == null)
            {
                return NotFound();
            }

            if(attachedFile.Task.UserCreatedId!= userId)
            {
                return Forbid();
            }

            attachedFile.Tittle = title;
            await context.SaveChangesAsync();
            return Ok();
        }
    }
}
