using System.Runtime.InteropServices;
using Windows.Win32.System.Variant;
using Windows.Win32.Foundation;


// ReSharper disable once CheckNamespace
namespace Windows.Win32.System.TaskScheduler;

// ReSharper disable once InconsistentNaming
internal unsafe partial struct ITaskFolder
{
    public void RegisterTaskDefinition(ITaskDefinition* task, string taskName)
    {
        var userId = string.IsNullOrEmpty(task->Principal->UserId.ToString())
            ? VARIANT.Null
            : VARIANT.FromBSTR(task->Principal->UserId);

        var logonType = task->Principal->LogonType;

        fixed (char* taskNamePtr = taskName)
        {
            IRegisteredTask* registeredTask;
            RegisterTaskDefinition((BSTR)taskNamePtr, task, (int)TASK_CREATION.TASK_CREATE_OR_UPDATE, 
                userId, VARIANT.Null, logonType, VARIANT.Null, &registeredTask);

            if (registeredTask == null)
                throw new COMException("Task registration error.");
        }
    }
}