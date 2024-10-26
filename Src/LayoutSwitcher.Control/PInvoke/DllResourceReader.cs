using System.Text;

namespace LayoutSwitcher.Control.PInvoke;

internal class DllResourceReader : IDisposable
{
    private readonly SafeLibraryHandle _libraryHandle;
    public DllResourceReader(string filename)
    {
        _libraryHandle = Kernel32.LoadLibraryExW(filename, IntPtr.Zero, Kernel32.LOAD_LIBRARY_AS_DATAFILE);
        if (_libraryHandle.IsInvalid)
            throw new ApplicationException($"Error loading {filename}.");
    }

    public string ReadStringResource(int id)
    {
        var stringLength = User32.LoadString(_libraryHandle, id, new StringBuilder(8), 0);
        if (stringLength == 0)
            throw new ArgumentException($"Resource {id} not found.");
        stringLength++; //Considering ending Null char
        var buffer = new StringBuilder(stringLength);
        _ = User32.LoadString(_libraryHandle, id, buffer, stringLength);

        return buffer.ToString();
    }

    #region Dispose

    private bool _disposed;

    ~DllResourceReader()
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
        if (!_libraryHandle.IsInvalid)
        {
            _libraryHandle.Close();
        }
        _disposed = true;
    }

    #endregion
}