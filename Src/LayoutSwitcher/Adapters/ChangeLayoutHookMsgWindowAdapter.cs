using LayoutControl;
using LayoutSwitcher.MessageWindow;

namespace LayoutSwitcher.Adapters;

public class ChangeLayoutHookMsgWindowAdapter : IMessageReceiver, IDisposable
{
    private readonly MsgOnlyWindow _msgWindow;

    public event EventHandler<WindowsMessageArgs>? WindowsMessageReceived;

    public ChangeLayoutHookMsgWindowAdapter(MsgOnlyWindow messageWindow)
    {
        _msgWindow = messageWindow;
        _msgWindow.MessageCaptured += OnMessageCaptured;
    }

    private void OnMessageCaptured(object? sender, MessageEventArgs e)
    {
        var args = new WindowsMessageArgs((uint)e.Msg, e.WParam, e.LParam);
        WindowsMessageReceived?.Invoke(this, args);
    }
    
    public IntPtr WindowHandle => _msgWindow.WndHandle;

    #region Dispose

    private bool _disposed;

    ~ChangeLayoutHookMsgWindowAdapter()
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
            _msgWindow.MessageCaptured -= OnMessageCaptured;
        }
        //free unmanaged resources (unmanaged objects) and override finalizer
        //set large fields to null

        _disposed = true;
    }

    #endregion
}