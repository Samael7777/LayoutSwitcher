using LayoutSwitcher.Models.Exceptions;

namespace LayoutSwitcher.Models.Tools;

public class SingleInstance(string appId, int checkTimeoutMs = 500) : IDisposable
{
    private readonly Mutex _appMutex = new(true, appId + "_mutex");

    public bool IsOtherInstancesPresents(bool exitContext = false)
    {
        return !_appMutex.WaitOne(checkTimeoutMs, exitContext);
    }

    public void CheckOtherInstancesThrowException()
    {
        if (IsOtherInstancesPresents())
            throw new ApplicationAlreadyRunningException();
    }

    #region Dispose

    private bool _disposed;

    ~SingleInstance()
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