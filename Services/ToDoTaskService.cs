

using Microsoft.EntityFrameworkCore;
using ProjetoTasks.Data;
using ProjetoTasks.Models;
using System.Diagnostics.Metrics;

namespace ProjetoTasks.Services
{
    internal class ToDoTaskService(ApplicationDbContext Context)
    {
        //private ApplicationDbContext _context;
        //public ToDoTaskService(ApplicationDbContext context)
        //{
        //    _context = context;
        //}

        public async Task AddTask(string title, string description)
        {
            var toDoTask = new ToDoTask(title, description);
            await Context.ToDoTasks.AddAsync(toDoTask);
            await Context.SaveChangesAsync();
        }
        public async Task EditTask(int id, string title, string description)
        {
            var todotask = await Context.ToDoTasks.FindAsync(id);
            if (todotask != null)
            {
                todotask.Title = title;
                todotask.Description = description;
                Context.Update(todotask);
                await Context.SaveChangesAsync();
            }
            else
            {
                throw new InvalidOperationException("Cannot edit a task that doesn't exist.");
            }
        }
        public async Task RemoveTask(int id)
        {
            var todotask = await Context.ToDoTasks.FindAsync(id);
            if (todotask != null)
            {
                Context.Remove(todotask);
                await Context.SaveChangesAsync();
            }
            else
            {
                throw new InvalidOperationException("Cannot remove a task that doesn't exist.");
            }
        }

        public async Task MarkAsComplete(int id)
        {
            var todotask = await Context.ToDoTasks.FindAsync(id);
            if (todotask != null)
            {
                todotask.IsCompleted = true;
                Context.Update(todotask);
                await Context.SaveChangesAsync();
            }
            else
            {
                throw new InvalidOperationException("Cannot complete a task that doesn't exist.");
            }
        }

        public async Task<List<ToDoTask>> GetAllTasks()
        {
            var allTasks = await Context.ToDoTasks.ToListAsync();
            return allTasks;
        }

        public async Task<bool> DoesExist(int id)
        {
            var todotask = await Context.ToDoTasks.FindAsync(id);
            return todotask != null;
        }
    }
}
