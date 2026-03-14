using Windows.Win32;
using Windows.Win32.System.LibraryLoader;

namespace LayoutSwitcher.Control.Tools;

internal class DllResourceReader : IDisposable
{
    private readonly FreeLibrarySafeHandle _libraryHandle;
    public DllResourceReader(string filename)
    {
        _libraryHandle = WinApi.LoadLibraryEx(filename, LOAD_LIBRARY_FLAGS.LOAD_LIBRARY_AS_DATAFILE);
        if (_libraryHandle.IsInvalid)
            throw new ApplicationException($"Error loading {filename}.");
    }

    public unsafe string ReadStringResource(uint id)
    {
        Span<char> buffer = stackalloc char[8];

        var stringLength = WinApi.LoadString(_libraryHandle, id, buffer, 0);
        if (stringLength == 0)
            throw new ArgumentException($"Resource {id} not found.");
        
        stringLength++; //Considering ending Null char
        buffer = stackalloc char[stringLength];
        
        _ = WinApi.LoadString(_libraryHandle, id, buffer, stringLength);

        return new string(buffer);
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