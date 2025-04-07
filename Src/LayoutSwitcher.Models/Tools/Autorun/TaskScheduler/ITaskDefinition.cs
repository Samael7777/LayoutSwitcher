
// ReSharper disable once CheckNamespace

using Windows.Win32.Foundation;

// ReSharper disable once CheckNamespace
namespace Windows.Win32.System.TaskScheduler;


// ReSharper disable once InconsistentNaming
internal unsafe partial struct ITaskDefinition
{
    public void SetDescription(string description)
    {
        fixed (char* descPtr = description)
        {
            RegistrationInfo->Description = (BSTR)descPtr;
        }
    }

    public void AddLogonTrigger()
    {
        ITrigger* trigger;
        Triggers->Create(TASK_TRIGGER_TYPE2.TASK_TRIGGER_LOGON, &trigger);
        trigger->Enabled = VARIANT_BOOL.VARIANT_TRUE;
    }

    public void AddExecAction(string cmdLine, string arguments)
    {
        fixed(char* cmdLinePtr = cmdLine)
        fixed (char* argsPtr = arguments)
        {
            IAction* action;
            Actions->Create(TASK_ACTION_TYPE.TASK_ACTION_EXEC, &action);
            ((IExecAction*)action)->Path = (BSTR)cmdLinePtr;
            ((IExecAction*)action)->Arguments = (BSTR)argsPtr;
        }
    }

    public void SetDefaultPrincipals(bool runAsAdmin)
    {
        Principal->LogonType = TASK_LOGON_TYPE.TASK_LOGON_INTERACTIVE_TOKEN;
        Principal->RunLevel = runAsAdmin 
            ? TASK_RUNLEVEL_TYPE.TASK_RUNLEVEL_HIGHEST 
            : TASK_RUNLEVEL_TYPE.TASK_RUNLEVEL_LUA;
    }

    public void SetDefaultSettings()
    {
        const string executionTimeLimitStr = "PT0S"; //No execution time limit

        fixed (char* executionTimeLimitStrPtr = executionTimeLimitStr)
        {
            Settings->DisallowStartIfOnBatteries = VARIANT_BOOL.VARIANT_FALSE;
            Settings->StopIfGoingOnBatteries = VARIANT_BOOL.VARIANT_FALSE;
            Settings->ExecutionTimeLimit = (BSTR)executionTimeLimitStrPtr;
        }
    }
}