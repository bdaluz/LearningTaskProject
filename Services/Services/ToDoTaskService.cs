using Microsoft.EntityFrameworkCore;
using Services.Data;
using Services.Models;
using System.Threading.Tasks;

namespace Services.Services
{
    public class ToDoTaskService(ApplicationDbContext Context) : IToDoTaskService
    {
        private async Task<ToDoTask?> GetToDoTask(int id)
        {
            return await Context.ToDoTasks.FindAsync(id);
        }

        public async Task<ToDoTask> AddTask(string title, string description, int userid)
        {
            var toDoTask = new ToDoTask(title, description);
            toDoTask.UserId = userid;
            await Context.ToDoTasks.AddAsync(toDoTask);
            await Context.SaveChangesAsync();
            return toDoTask;
        }

        public async Task EditTask(int id, string title, string description)
        {
            var todotask = await GetToDoTask(id);
            if (todotask == null) throw new InvalidOperationException("Task not found.");
            todotask.Title = title;
            todotask.Description = description;
            Context.Update(todotask);
            await Context.SaveChangesAsync();
        }
        public async Task RemoveTask(int id)
        {
            var todotask = await GetToDoTask(id);
            if (todotask == null) throw new InvalidOperationException("Task not found.");
            Context.Remove(todotask);
            await Context.SaveChangesAsync();
        }

        public async Task MarkAsComplete(int id)
        {
            var todotask = await GetToDoTask(id);
            if (todotask == null) throw new InvalidOperationException("Task not found.");
            todotask.IsCompleted = !todotask.IsCompleted;
            Context.Update(todotask);
            await Context.SaveChangesAsync();
        }

        public Task<List<ToDoTask>> GetAllUserTasks(int userid)
        {
            return Context.ToDoTasks.Where(x => x.UserId == userid).ToListAsync();
        }

        public Task<bool> TaskBelongsToUser(int id, int userid)
        {
            return Context.ToDoTasks.AnyAsync(x => x.UserId == userid && x.Id == id);
        }
    }
}
