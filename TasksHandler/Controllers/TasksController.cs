using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TasksHandler.Entities;
using TasksHandler.Services;

namespace TasksHandler.Controllers
{
    [Route("api/Tasks")]
    public class TasksController : ControllerBase
    {
        private readonly ApplicationDbContext applicationDbContext;
        private readonly IUsersService usersService;

        public TasksController(ApplicationDbContext applicationDbContext, 
            IUsersService usersService)
        {
            this.applicationDbContext = applicationDbContext;
            this.usersService = usersService;
        }

        [HttpGet]
        public async Task<List<Tasks>> Get()
        {
            return await applicationDbContext.Tasks.ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<Tasks>> Post([FromBody] string title)
        {
            var userId = usersService.getUserId();

            var tasksExists = await applicationDbContext.Tasks.AnyAsync(t => t.UserCreatedId == userId);

            var mayorOrden = 0;
            if (tasksExists)
            {
                mayorOrden = await applicationDbContext.Tasks.Where(t => t.UserCreatedId == userId)
                    .Select(t => t.Orden).MaxAsync();
            }

            var task = new Tasks
            {
                Title = title,
                UserCreatedId = userId,
                Creationdate = DateTime.UtcNow,
                Orden = mayorOrden + 1
            };

            applicationDbContext.Add(task);
            await applicationDbContext.SaveChangesAsync();
            return task;
        }
    }
}
