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

    while (true)
    {
        Console.WriteLine("Menu:");
        Console.WriteLine("1 - Add a task");
        Console.WriteLine("2 - Edit a task");
        Console.WriteLine("3 - Delete a task");
        Console.WriteLine("4 - Mark task as completed");
        Console.WriteLine("5 - Show all tasks");
        Console.Write("\nSelect an option: ");
        try
        {
            int choice = int.Parse(Console.ReadLine());
            if (new List<int> { 1, 2, 3, 4, 5 }.Contains(choice))
            {
                Console.WriteLine();
                switch (choice)
                {
                    case 1:
                        await AddTask(todoservice);
                        break;
                    case 2:
                        await EditTask(todoservice);
                        break;
                    case 3:
                        await RemoveTask(todoservice);
                        break;
                    case 4:
                        await CompleteTask(todoservice);
                        break;
                    case 5:
                        await ShowTasks(todoservice);
                        break;
                }
            }
            else
            {
                Console.WriteLine("\nInvalid option.\n");
            }
        }
        catch (Exception ex) { Console.WriteLine("\nInvalid option.\n"); }
    }
}

static async Task AddTask(ToDoTaskService todoservice)
{
    Console.WriteLine("Add a task\n");
    Console.Write("Title: ");
    string title = Console.ReadLine().Trim();
    Console.Write("Description: ");
    string description = Console.ReadLine().Trim();
    Console.WriteLine();
    await todoservice.AddTask(title, description);
    Console.WriteLine("New task added successfully.");
}


static async Task EditTask(ToDoTaskService todoservice)
{
    Console.Clear();
    await ShowTasks(todoservice);
    Console.WriteLine();
    Console.WriteLine("Edit a task\n");
    Console.Write("Enter the ID of the task you'd like to edit: ");
    string id = Console.ReadLine().Trim();
    try
    {
        if (!await todoservice.DoesExist(int.Parse(id)))
        {
            Console.WriteLine("Cannot edit a task that doesn't exist.\n");
            return;
        }

    }
    catch (FormatException e)
    {
        Console.WriteLine(e.Message);
        return;
    }
    Console.Write("New Title: ");
    string title = Console.ReadLine().Trim();
    Console.Write("New Description: ");
    string description = Console.ReadLine().Trim();
    Console.WriteLine();
    try
    {
        await todoservice.EditTask(int.Parse(id), title, description);
        Console.WriteLine("Task was edited successfully.");
    }
    catch (InvalidOperationException e)
    {
        Console.WriteLine(e.Message);
    }
    catch (Exception e)
    {
        Console.WriteLine(e.Message);
    }
}

static async Task RemoveTask(ToDoTaskService todoservice)
{
    Console.Clear();
    await ShowTasks(todoservice);
    Console.WriteLine();
    Console.WriteLine("Edit a task\n");
    Console.Write("Enter the ID of the task you'd like to remove: ");
    string id = Console.ReadLine().Trim();
    try
    {
        if (!await todoservice.DoesExist(int.Parse(id)))
        {
            Console.WriteLine("Cannot remove a task that doesn't exist.\n");
            return;
        }
        else { 
            await todoservice.RemoveTask(int.Parse(id));
        }

    }
    catch (FormatException e)
    {
        Console.WriteLine(e.Message);
        return;
    }
    catch (InvalidOperationException e)
    {
        Console.WriteLine(e.Message);
    }
    catch (Exception e)
    {
        Console.WriteLine(e.Message);
    }
}

static async Task CompleteTask(ToDoTaskService todoservice)
{
    Console.Clear();
    await ShowTasks(todoservice);
    Console.WriteLine();
    Console.WriteLine("Complete a task\n");
    Console.Write("Enter the ID of the task you'd like to mark as completed: ");
    string id = Console.ReadLine().Trim();
    try
    {
        if (!await todoservice.DoesExist(int.Parse(id)))
        {
            Console.WriteLine("Cannot complete a task that doesn't exist.\n");
            return;
        }
        else
        {
            await todoservice.MarkAsComplete(int.Parse(id));
        }

    }
    catch (FormatException e)
    {
        Console.WriteLine(e.Message);
        return;
    }
    catch (InvalidOperationException e)
    {
        Console.WriteLine(e.Message);
    }
    catch (Exception e)
    {
        Console.WriteLine(e.Message);
    }
}


static async Task ShowTasks(ToDoTaskService todoservice)
{
    Console.WriteLine("All tasks:\n");
    var tasks = await todoservice.GetAllTasks();
    if (tasks.Any())
    {
        foreach (var task in tasks)
        {
            Console.WriteLine($"Id: {task.Id}, \nTitle: {task.Title}\nDescription: {task.Description}\nStatus: {(task.IsCompleted ? "Completed" : "Pending")}\n");
        }
    }
    else
    {
        Console.WriteLine("No task found.\n");
    }
}