using Services.Models;
public interface IToDoTaskService
{
    Task<ToDoTask> AddTask(string title, string description, int userid);
    Task EditTask(int id, string title, string description);
    Task RemoveTask(int id);
    Task MarkAsComplete(int id);
    Task<List<ToDoTask>> GetAllUserTasks(int userid);
    Task<bool> TaskBelongsToUser(int id, int userid);

}