namespace LayoutSwitcher.Models.Tools;

internal interface IAutorun
{
    bool IsAutorunEnabled(string appId);
    void AddToAutoRun(string appId, string appPath);
    void RemoveFromAutorun(string appId);
}