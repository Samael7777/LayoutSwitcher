using LayoutSwitcher.Models.Tools;

namespace LayoutSwitcher.Models;

public class AutorunModel(string appId, string appPath)
{
    private readonly IAutorun _autorun = new AutorunTaskScheduler();

    public bool Autorun
    {
        get => _autorun.IsAutorunEnabled(appId);
        set
        {
            if (value == _autorun.IsAutorunEnabled(appId)) 
                return;

            if (value)
            {
                _autorun.AddToAutoRun(appId, appPath);
            }
            else
            {
                _autorun.RemoveFromAutorun(appId);
            }
        }
    }
}