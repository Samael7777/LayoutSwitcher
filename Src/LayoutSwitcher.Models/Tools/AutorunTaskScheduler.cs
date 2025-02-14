using Microsoft.Win32.TaskScheduler;

namespace LayoutSwitcher.Models.Tools;

public class AutorunTaskScheduler : IAutorun
{
    public bool IsAutorunEnabled(string appId)
    {
        using var ts = new TaskService();
        
        var taskPath = $@"\{appId}";
        var task = ts.GetTask(taskPath);

        return task != null;
    }

    public void AddToAutoRun(string appId, string appPath)
    {
        using var ts = new TaskService();
        
        var td = ts.NewTask();
        td.RegistrationInfo.Description = $"{appId} autorun task";
        td.Triggers.Add(new LogonTrigger());
        td.Actions.Add(appPath);

        //Run with admin privileges
        td.Principal.LogonType = TaskLogonType.InteractiveToken;
        td.Principal.RunLevel = TaskRunLevel.Highest;

        td.Settings.DisallowStartIfOnBatteries = false;
        td.Settings.StopIfGoingOnBatteries = false;
        ts.RootFolder.RegisterTaskDefinition(appId, td);
    }

    public void RemoveFromAutorun(string appId)
    {
        using var ts = new TaskService();
        ts.RootFolder.DeleteTask(appId, false);
    }
}