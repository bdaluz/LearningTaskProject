using Microsoft.EntityFrameworkCore;
using Services.Data;
using Services.Models;

namespace Services.Services
{
    public class ToDoTaskService(ApplicationDbContext Context) : IToDoTaskService
    {
        private async Task<ToDoTask> GetToDoTask(int id)
        {
            var todotask = await Context.ToDoTasks.FindAsync(id);
            if (todotask != null)
            {
                return todotask;
            }
            throw new InvalidOperationException("Task not found.");
        }
        public async Task AddTask(string title, string description, int userid)
        {
            var toDoTask = new ToDoTask(title, description);
            toDoTask.UserId = userid;
            await Context.ToDoTasks.AddAsync(toDoTask);
            await Context.SaveChangesAsync();
        }
        public async Task EditTask(int id, string title, string description)
        {
            var todotask = await GetToDoTask(id);
            todotask.Title = title;
            todotask.Description = description;
            Context.Update(todotask);
            await Context.SaveChangesAsync();
        }
        public async Task RemoveTask(int id)
        {
            var todotask = await GetToDoTask(id);
            Context.Remove(todotask);
            await Context.SaveChangesAsync();
        }

        public async Task MarkAsComplete(int id)
        {
            var todotask = await GetToDoTask(id);
            todotask.IsCompleted = true;
            Context.Update(todotask);
            await Context.SaveChangesAsync();
        }

        public Task<List<ToDoTask>> GetAllUserTasks(int userid)
        {
            return Context.ToDoTasks.Where(x => x.UserId == userid).ToListAsync();
        }

        public Task<bool> UserDoesExist(int id, int userid)
        {
            //var todotask = await Context.ToDoTasks.FindAsync(id);
            return Context.ToDoTasks.AnyAsync(x => x.UserId == userid && x.Id == id);
        }

        public Task<bool> TaskDoesExist(int userid)
        {
            return Context.ToDoTasks.AnyAsync(x => x.UserId == userid);
        }
    }
}
