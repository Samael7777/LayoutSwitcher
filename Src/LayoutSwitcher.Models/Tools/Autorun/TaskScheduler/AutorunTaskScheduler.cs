namespace LayoutSwitcher.Models.Tools.Autorun.TaskScheduler;

public class AutorunTaskScheduler : IAutorun
{
    private const string TaskSchedulerFolder = @"\";

    public bool IsAutorunEnabled(string appId)
    {
        using var taskService = new TaskService();
        var services = taskService.GetTasks(TaskSchedulerFolder);

        return services.Any(s=>string.Equals(appId, s, StringComparison.OrdinalIgnoreCase));
    }

    public void AddToAutoRun(string appId, string appPath)
    {
        using var taskService = new TaskService();

        var taskDesc = $"{appId} autorun record.";
        taskService.AddTask(TaskSchedulerFolder, appId, taskDesc, 
            appPath, "", true);
    }

    public void RemoveFromAutorun(string appId)
    {
        using var taskService = new TaskService();
        taskService.DeleteTask(TaskSchedulerFolder, appId);
    }
}