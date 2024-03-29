﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TasksHandler.Entities;
using TasksHandler.Models;
using TasksHandler.Services;

namespace TasksHandler.Resources.Controllers
{
    [Route("api/Steps")]
    public class StepsController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly IUsersService usersService;

        public StepsController(ApplicationDbContext context, IUsersService usersService)
        {
            this.context = context;
            this.usersService = usersService;
        }
        [HttpPost("{taskId:int}")]
        public async Task<ActionResult<Steps>> Post (int taskId, [FromBody] CreateStepDTO createStepDTO)
        {
            var userId = usersService.getUserId();

            var task = await context.Tasks.FirstOrDefaultAsync(t => t.Id== taskId);

            if(task == null)
            {
                return NotFound();
            }

            if(task.UserCreatedId != userId) {
                return Forbid();

            }

            var thereAreSteps = await context.Steps.AnyAsync(p => p.TasksId==taskId);

            var orderGreater = 0;

            if(thereAreSteps)
            {
                orderGreater = await context.Steps.Where
                    (p => p.TasksId == taskId).Select(p => p.Orden).MaxAsync();
            }

            var step = new Steps();
            step.TasksId = taskId;
            step.Orden = orderGreater + 1;
            step.Description = createStepDTO.Description;
            step.Done = createStepDTO.Done;

            context.Add(step);
            await context.SaveChangesAsync();

            return step;
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Put(Guid id, [FromBody] CreateStepDTO createStepDTO)
        {
            var userId = usersService.getUserId();

            var step = await context.Steps.Include(p => p.task).FirstOrDefaultAsync(p => p.Id== id);

            if(step is null) { 
                return NotFound(); 
            }

            if(step.task.UserCreatedId != userId)
            {
                return Forbid();
            }

            step.Description = createStepDTO.Description;
            step.Done = createStepDTO.Done;

            await context.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var userId = usersService.getUserId();

            var step = await context.Steps.Include(p => p.task).FirstOrDefaultAsync(p => p.Id == id);

            if (step is null)
            {
                return NotFound();
            }

            if(step.task.UserCreatedId!= userId)
            {
                return Forbid();
            }

            context.Steps.Remove(step);
            await context.SaveChangesAsync();

            return Ok();
        }
        [HttpPost("order/{taskId:int}")]
        public async Task<IActionResult> Order(int taskId, [FromBody] Guid [] ids)
        {
            var userId = usersService.getUserId();

            var task = await context.Tasks.FirstOrDefaultAsync(t => t.Id== taskId
            && t.UserCreatedId == userId);

            if(task is null)
            {
                return NotFound();
            }

            var steps = await context.Steps.Where(x => x.TasksId==taskId).ToListAsync();

            var stepsIds = steps.Select(x => x.Id);

            var setpsIdsNotBelongToTask = ids.Except(stepsIds).ToList();

            if(setpsIdsNotBelongToTask.Any())
            {
                return BadRequest("Not all steps are present");
            }

            var stepsDictionary = steps.ToDictionary(p => p.Id);

            for(int i=0; i<ids.Length; i++)
            {
                var stepId = ids[i];
                var step = stepsDictionary[stepId];
                step.Orden = i + 1;
            }

            await context.SaveChangesAsync();
            return Ok();
        }
    }
}
