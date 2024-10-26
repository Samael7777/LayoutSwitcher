using LayoutSwitcher.Models.Tools;

namespace LayoutSwitcher.Models;

public class AutorunModel
{
    private readonly string _appId;
    private readonly string _appPath;

    public AutorunModel(string appId, string appPath)
    {
        _appId = appId;
        _appPath = appPath;
    }
    
    public bool Autorun
    {
        get => AutorunHelper.IsAutorunEnabled(_appId);
        set
        {
            if (value == AutorunHelper.IsAutorunEnabled(_appId)) 
                return;

            if (value)
            {
                AutorunHelper.AddToAutoRun(_appId, _appPath);
            }
            else
            {
                AutorunHelper.RemoveFromAutorun(_appId);
            }
        }
    }
}