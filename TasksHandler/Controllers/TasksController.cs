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
