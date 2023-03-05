using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TasksHandler.Entities;
using TasksHandler.Models;
using TasksHandler.Services;

namespace TasksHandler.Controllers
{
    [Route("api/Tasks")]
    public class TasksController : ControllerBase
    {
        private readonly ApplicationDbContext applicationDbContext;
        private readonly IUsersService usersService;
        private readonly IMapper mapper;

        public TasksController(
            ApplicationDbContext applicationDbContext, 
            IUsersService usersService,
            IMapper mapper)
        {
            this.applicationDbContext = applicationDbContext;
            this.usersService = usersService;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<List<TaskDTO>> Get()
        {
            var UserId = usersService.getUserId();
            var tasks = await applicationDbContext.Tasks.Where(t => t.UserCreatedId == UserId)
                .OrderBy(t => t.Orden)
                .ProjectTo<TaskDTO>(mapper.ConfigurationProvider)
                .ToListAsync();

            return tasks;
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Tasks>> Get(int id)
        {
            var UserId = usersService.getUserId();

            var task = await applicationDbContext.Tasks.FirstOrDefaultAsync(t => t.Id== id 
            && t.UserCreatedId == UserId);

            if(task is null)
            {
                return NotFound();
            }

            return task;
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

        [HttpPut("{id:int}")]
        public async Task<IActionResult> EditTask(int id, [FromBody] EditTaskDTO editTaskDTO)
        {
            var UserId = usersService.getUserId();

            var task = await applicationDbContext.Tasks.FirstOrDefaultAsync(t => t.Id == id
            && t.UserCreatedId == UserId);

            if(task is null)
            {
                return NotFound();
            }

            task.Title= editTaskDTO.Title;
            task.Description= editTaskDTO.Description;
            await applicationDbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("sort")]
        public async Task<IActionResult> Sort([FromBody] int[] ids)
        {
            var userId = usersService.getUserId();

            var tasks = await applicationDbContext.Tasks
                .Where(t => t.UserCreatedId == userId).ToListAsync();

            var tasksId = tasks.Select(t => t.Id);

            var idsTasksNotBelongToUser = ids.Except(tasksId).ToList();

            if(idsTasksNotBelongToUser.Any())
            {
                return Forbid();
            }

            var tasksDictionary = tasks.ToDictionary(x => x.Id);

            for(int i = 0; i<ids.Length; i++)
            {
                var id = ids[i];
                var task = tasksDictionary[id];
                task.Orden = i + 1;
            }

            await applicationDbContext.SaveChangesAsync();

            return Ok();
        }
    }
}
