using ProjetoTasks.Models;
using ProjetoTasks.Services;


namespace ProjetoTasks.Menus
{
    internal class ToDoTaskMenu
    {
        public async Task Menu(ToDoTaskService todoservice, int userid)
        {
            await ShowTasks(todoservice, userid);

            while (true)
            {
                //Console.BackgroundColor = ConsoleColor.Blue;
                //Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Menu:");
                //Console.ResetColor();
                Console.WriteLine("1 - Add a task");
                Console.WriteLine("2 - Edit a task");
                Console.WriteLine("3 - Delete a task");
                Console.WriteLine("4 - Mark task as completed");
                Console.WriteLine("5 - Show all tasks");
                Console.WriteLine("6 - Logout");
                Console.Write("\nSelect an option: ");
                try
                {
                    int choice = int.Parse(Console.ReadLine());
                    Console.WriteLine();
                    switch (choice)
                    {
                        case 1:
                            await AddTask(todoservice, userid);
                            break;
                        case 2:
                            await EditTask(todoservice, userid);
                            break;
                        case 3:
                            await RemoveTask(todoservice, userid);
                            break;
                        case 4:
                            await CompleteTask(todoservice, userid);
                            break;
                        case 5:
                            await ShowTasks(todoservice, userid);
                            break;
                        case 6:
                            return;
                        default:
                            Console.WriteLine("\nInvalid option.\n");
                            continue;
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("\nInvalid option.\n");
                }
            }
        }

        static async Task AddTask(ToDoTaskService todoservice, int userid)
        {
            Console.WriteLine("Add a task\n");
            Console.Write("Title: ");
            string title = Console.ReadLine().Trim();
            Console.Write("Description: ");
            string description = Console.ReadLine().Trim();
            Console.WriteLine();
            await todoservice.AddTask(title, description, userid);
            Console.WriteLine("New task added successfully.");
        }


        static async Task EditTask(ToDoTaskService todoservice, int userid)
        {
            await ShowTasks(todoservice, userid);
            Console.WriteLine();
            Console.WriteLine("Edit a task\n");
            Console.Write("Enter the ID of the task you'd like to edit: ");
            string id = Console.ReadLine().Trim();
            try
            {
                if (!await todoservice.DoesExist(int.Parse(id), userid))
                {
                    Console.WriteLine("\nCannot edit a task that doesn't exist.\n");
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

        static async Task RemoveTask(ToDoTaskService todoservice, int userid)
        {
            await ShowTasks(todoservice, userid);
            Console.WriteLine();
            Console.WriteLine("Edit a task\n");
            Console.Write("Enter the ID of the task you'd like to remove: ");
            string id = Console.ReadLine().Trim();
            try
            {
                if (!await todoservice.DoesExist(int.Parse(id), userid))
                {
                    Console.WriteLine("\nCannot remove a task that doesn't exist.\n");
                    return;
                }
                else
                {
                    await todoservice.RemoveTask(int.Parse(id));
                    Console.WriteLine("Task deleted successfully.");
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

        static async Task CompleteTask(ToDoTaskService todoservice, int userid)
        {
            await ShowTasks(todoservice, userid);
            Console.WriteLine();
            Console.WriteLine("Complete a task\n");
            Console.Write("Enter the ID of the task you'd like to mark as completed: ");
            string id = Console.ReadLine().Trim();
            try
            {
                if (!await todoservice.DoesExist(int.Parse(id), userid))
                {
                    Console.WriteLine("\nCannot complete a task that doesn't exist.\n");
                    return;
                }
                else
                {
                    await todoservice.MarkAsComplete(int.Parse(id));
                    await ShowTasks(todoservice, userid);
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


        static async Task ShowTasks(ToDoTaskService todoservice, int userid)
        {
            Console.WriteLine("All tasks:\n");
            var tasks = await todoservice.GetAllUserTasks(userid);
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
    }
}
