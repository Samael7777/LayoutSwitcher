using LayoutSwitcher.Models.Exceptions;
using LayoutSwitcher.Models.Interfaces;
using LayoutSwitcher.Models.Tools;

namespace LayoutSwitcher.Models;

public class AppModel : IDisposable
{
    private const string AppRegistryKey = @"Vadim Kutin\Layout Switcher";

    public const string AppId = "LayoutSwitcher";

    private readonly SystemSettingsWatcher _systemSettingsChangesWatcher;
    private readonly SettingsInRegistry _settings;

    public CycledLayoutsModel CycledLayoutsModel { get; }
    public IHotKeyModel HotKeyModel { get; }
    public AutorunModel AutorunModel { get; }

   
    public AppModel(IHotKeyModel hotKeyModel, string appPath)
    {
        HotKeyModel = hotKeyModel;
        AutorunModel = new AutorunModel(AppId, appPath);
        InitSettings(out _settings);
        CycledLayoutsModel = new CycledLayoutsModel(_settings.CycledLayout);

        SetupHotkeys(hotKeyModel, _settings, CycledLayoutsModel);
        
        InitSystemSettingsWatcher(CycledLayoutsModel, out _systemSettingsChangesWatcher);
    }
    
    private static void InitCheckSingleAppInstance(out SingleInstance instance)
    {
        instance = new SingleInstance(AppId);
        instance.CheckOtherInstancesThrowException();
    }

    private static void InitSettings(out SettingsInRegistry settings)
    {
        settings = new SettingsInRegistry(AppRegistryKey);
        settings.Load();
    }

    private void SetupHotkeys(IHotKeyModel hotKeyModel, SettingsInRegistry settings, CycledLayoutsModel cycledLayoutsModel)
    {
        hotKeyModel.HotKeyIndex = settings.LayoutToggleHotKeyIndex;
        hotKeyModel.HotKeyPressed += (_, _) => cycledLayoutsModel.SwitchToNextLayout();
    }

    private static void InitSystemSettingsWatcher(CycledLayoutsModel cycledLayoutsModel, out SystemSettingsWatcher watcher)
    {
        watcher = new SystemSettingsWatcher();
        watcher.SystemLayoutsChanged += (_, _) => cycledLayoutsModel.CleanFromOrphanedLayouts();
    }

    public void SaveSettings()
    {
        _settings.CycledLayout = CycledLayoutsModel.CycledLayouts;
        _settings.LayoutToggleHotKeyIndex = HotKeyModel.HotKeyIndex;
        _settings.Save();
    }
    
    #region Dispose

    private bool _disposed;

    ~AppModel()
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
            _systemSettingsChangesWatcher.Dispose();
        }
        //free unmanaged resources (unmanaged objects) and override finalizer
        //set large fields to null

        _disposed = true;
    }

    #endregion
}