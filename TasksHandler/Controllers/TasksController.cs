using Microsoft.AspNetCore.Mvc;
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
    }
}
