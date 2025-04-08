using ProjetoTasks.Menus;

namespace ProjetoTasks
{
    class RunProgram(ToDoTaskMenu taskMenu, UserMenu userMenu)
    {
        public async Task Run()
        {
            while (true)
            {
                int userid = await userMenu.Menu();
                await taskMenu.Menu(userid);
            }
        }
    }
}
