using ProjetoTasks.Data;
using ProjetoTasks.Menus;
using ProjetoTasks.Models;
using ProjetoTasks.Services;


RunProgram().Wait();



async Task RunProgram()
{
    var context = new ApplicationDbContext();
    var todoservice = new ToDoTaskService(context);
    var userservice = new UserService(context);

    while (true) { 
    UserMenu userMenu = new UserMenu();
    int userid = await userMenu.Menu(userservice);

    ToDoTaskMenu taskMenu = new ToDoTaskMenu();

    await taskMenu.Menu(todoservice, userid);
    }
}

