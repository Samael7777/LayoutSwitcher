using System.Runtime.InteropServices;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.System.Com;
using Windows.Win32.System.TaskScheduler;
using Windows.Win32.System.Variant;

namespace LayoutSwitcher.Models.Tools.Autorun.TaskScheduler;

internal unsafe  class TaskService : IDisposable
{
    private readonly Guid _taskSchedulerClsId = new("0F87369F-A4E5-4CFC-BD3E-73E6154572DD");

    private readonly ITaskService* _taskService;

    public TaskService()
    {
        WinApi.CoInitializeEx(COINIT.COINIT_APARTMENTTHREADED);
        ConnectToTaskScheduler(out _taskService);
    }

    public List<string> GetTasks(string taskSchedulerFolderPath)
    {
        var list = new List<string>();
        var folder = GetTaskSchedulerFolder(taskSchedulerFolderPath);
        
        IRegisteredTaskCollection* tasks;
        folder->GetTasks(1, &tasks);
        
        for (var i = 1; i <= tasks->Count; i++)
        {
            IRegisteredTask* item;
            tasks->get_Item(VARIANT.FromInt(i), &item);

            list.Add(item->Name.ToString());
        }

        return list;
    }

    public List<string> GetFolders(string baseFolderPath)
    {
        var list = new List<string>();
        var folder = GetTaskSchedulerFolder(baseFolderPath);

        ITaskFolderCollection* folders;
        folder->GetFolders(0, &folders);

        for (var i = 1; i <= folders->Count; i++)
        {
            ITaskFolder* item;
            folders->get_Item(VARIANT.FromInt(i), &item);

            list.Add(item->Name.ToString());
        }

        return list;
    }

    public void AddTask(string taskSchedulerFolderPath, string name, string description, string cmdLine, string args, bool runAsAdmin)
    {
        var folder = GetTaskSchedulerFolder(taskSchedulerFolderPath);

        ITaskDefinition* newTask;
        _taskService->NewTask(0, &newTask);

        newTask->SetDescription(description);
        newTask->AddLogonTrigger();
        newTask->AddExecAction(cmdLine, args);
        newTask->SetDefaultPrincipals(runAsAdmin);
        newTask->SetDefaultSettings();

        folder->RegisterTaskDefinition(newTask, name);
    }

    public void DeleteTask(string taskSchedulerFolderPath, string name)
    {
        var folder = GetTaskSchedulerFolder(taskSchedulerFolderPath);
        fixed (char* namePtr = name)
        {
            folder->DeleteTask((BSTR)namePtr, 0);
        }
    }

    private void ConnectToTaskScheduler(out ITaskService* taskService)
    {
        var iid = ITaskService.IID_Guid;
        WinApi.CoCreateInstance(in _taskSchedulerClsId, null,
                CLSCTX.CLSCTX_INPROC_SERVER, in iid, out var pUnk)
            .ThrowOnFailure();

        taskService = (ITaskService*)pUnk;
        _taskService->Connect(VARIANT.Null, VARIANT.Null, VARIANT.Null, VARIANT.Null);

        if (_taskService->Connected.Value == 0)
            throw new COMException("Task Scheduler Service connection error.");
    }

    private ITaskFolder* GetTaskSchedulerFolder(string path)
    {
        ITaskFolder* folderPtr;
        fixed(char* pathPtr = path)
        {
            _taskService->GetFolder((BSTR)pathPtr, &folderPtr);
        }

        return folderPtr;
    }

    #region Dispose

    private bool _disposed;

    ~TaskService()
    {
        Dispose(false);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;
        if (disposing)
        {
            //dispose managed state (managed objects)
            
        }
        //free unmanaged resources (unmanaged objects) and override finalizer
        //set large fields to null
        WinApi.CoUninitialize();

        _disposed = true;
    }

    #endregion
}