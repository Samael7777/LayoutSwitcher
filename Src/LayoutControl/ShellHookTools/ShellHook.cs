using System.Diagnostics;
using LayoutControl.PInvoke;

// ReSharper disable InconsistentNaming

namespace LayoutControl.ShellHookTools;

[DebuggerDisplay("{_hook.handle.ToInt32()}")]
public class ShellHook : IDisposable
{
   
    private readonly IMessageReceiver _msgWindow;
    private readonly SafeShellHookHandle _hook;
    private readonly uint _shellHookMessageCode;
    private KeyboardLayout _currentLayout;

    public event EventHandler<LayoutChangedEventArgs>? LayoutChanged;

    public ShellHook(IMessageReceiver messageReceiver)
    {
        _msgWindow = messageReceiver;
        if (_msgWindow.WindowHandle == IntPtr.Zero)
            throw new ApplicationException("Can't get message window handler.");

		_msgWindow.WindowsMessageReceived += OnMessageCaptured;

        _currentLayout = KeyboardLayoutInfo.GetForegroundWindowLayout();

        _shellHookMessageCode = ShellHookNative.GetShellHookMessageCode();
        _hook = ShellHookNative.RegisterShellHook(_msgWindow.WindowHandle);
    }

    private void OnMessageCaptured(object? sender, WindowsMessageArgs e)
    {
        var message = e.Msg;
        var lParam = e.LParam;

        if (message != _shellHookMessageCode || lParam == IntPtr.Zero) return;
		
		#if DEBUG
		Debug.WriteLine($"ShellHook: Layout changed to {lParam.ToInt32():x8}");
		#endif
		InvokeLayoutChanged((uint)lParam.ToInt32());

		//Template for fix bug, when new HKL is in higher bits of LParam  
		//uint newHkl;
		//if (IntPtr.Size == 4)
		//{
		//    newHkl = (uint)lParam;
		//}
		//else
		//{
		//    var lParamHigh = (uint)(lParam.ToInt64() >> 32);
		//    var lParamLow = (uint)(lParam.ToInt64() & 0xFFFFFFFF);
		//    newHkl = lParamHigh is 0 or 0xFFFFFFFF ? lParamLow : lParamHigh;
		//}
    }

    private void InvokeLayoutChanged(uint hkl)
    {
        if (hkl == _currentLayout.Hkl) return;

        _currentLayout = KeyboardLayout.GetLayout(hkl);
#if DEBUG
        Debug.WriteLine($"Layout changed to {_currentLayout.LayoutDisplayName}");
#endif
        var args = new LayoutChangedEventArgs(_currentLayout);
        LayoutChanged?.Invoke(this, args);
    }
	
    #region Dispose

    private bool _disposed;

    ~ShellHook()
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
            _msgWindow.WindowsMessageReceived -= OnMessageCaptured;
            _hook.Dispose();
        }
        //free unmanaged resources (unmanaged objects) and override finalizer
        //set large fields to null

        _disposed = true;
    }

    #endregion
}