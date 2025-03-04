using ProjetoTasks.Models;
using ProjetoTasks.Data;
using ProjetoTasks.Services;
using System.Threading.Tasks;

RunProgram().Wait();



async Task RunProgram()
{
    var context = new ApplicationDbContext();
    var todoservice = new ToDoTaskService(context);

    await ShowTasks(todoservice);

    Console.WriteLine("\nAdd a task\n");
    Console.Write("Title: ");
    string title = Console.ReadLine();
    Console.Write("Description: ");
    string description = Console.ReadLine();

    await todoservice.AddTask(title, description);
    await ShowTasks(todoservice);
}

static async Task ShowTasks(ToDoTaskService todoservice)
{
    Console.WriteLine("All tasks:");
    var tasks = await todoservice.GetAllTasks();
    if (tasks.Any())
    {
        foreach (var task in tasks)
        {
            Console.WriteLine($"Id: {task.Id}, \nTitle: {task.Title}\nDescription: {task.Description}\nStatus: {(task.IsCompleted ? "Completed" : "Pending")}");
        }
    }
    else {
        Console.WriteLine("No task found.\n");
    }
}