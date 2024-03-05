using HotKeyHook;
using LayoutSwitcher.MessageWindow;

namespace LayoutSwitcher.Adapters;

public class HotKeyHookMsgWindowAdapter : IMessageReceiver, IDisposable
{
    private readonly MsgOnlyWindow _msgOnlyWindow;

    public event EventHandler<WindowsMessageArgs>? WindowsMessageReceived;

    public HotKeyHookMsgWindowAdapter(MsgOnlyWindow messageWindow)
    {
        _msgOnlyWindow = messageWindow;
        _msgOnlyWindow.MessageCaptured += OnMessageCaptured;
    }

    public IntPtr WindowHandle => _msgOnlyWindow.WndHandle;

    public void Invoke(Action action)
    {
        _msgOnlyWindow.Invoke(action);
    }

    private void OnMessageCaptured(object? sender, MessageEventArgs messageEventArgs)
    {
        var args = new WindowsMessageArgs((uint)messageEventArgs.Msg, messageEventArgs.WParam, messageEventArgs.LParam);
        WindowsMessageReceived?.Invoke(this, args);
    }

    #region Dispose

    private bool _disposed;

    ~HotKeyHookMsgWindowAdapter()
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
            _msgOnlyWindow.MessageCaptured -= OnMessageCaptured;
        }
        //free unmanaged resources (unmanaged objects) and override finalizer
        //set large fields to null

        _disposed = true;
    }

    #endregion
}