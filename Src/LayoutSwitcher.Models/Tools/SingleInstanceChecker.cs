using LayoutSwitcher.Models.Exceptions;

namespace LayoutSwitcher.Models.Tools;

public class SingleInstanceChecker : IDisposable
{
    private readonly Mutex _appMutex;
    private readonly int _checkTimeoutMs;

    public SingleInstanceChecker(string appId, int checkTimeoutMs = 500)
    {
        _appMutex = new Mutex(true, appId + "_mutex");
        _checkTimeoutMs = checkTimeoutMs;
    }

    public bool IsOtherInstancesPresents(bool exitContext = false)
    {
        return !_appMutex.WaitOne(_checkTimeoutMs, exitContext);
    }

    public void CheckOtherInstancesThrowException()
    {
        if (IsOtherInstancesPresents())
            throw new ApplicationAlreadyRunningException();
    }

    #region Dispose

    private bool _disposed;

    ~SingleInstanceChecker()
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
            _appMutex.ReleaseMutex();
            _appMutex.Dispose();
        }
        //free unmanaged resources (unmanaged objects) and override finalizer
        //set large fields to null

        _disposed = true;
    }

    #endregion
}