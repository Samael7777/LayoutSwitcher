using System.Runtime.InteropServices;
using LayoutControl.PInvoke;
using LayoutControl.Wrapper;

// ReSharper disable InconsistentNaming

namespace LayoutControl;


public class ChangeLayoutHook : IDisposable
{
    private const string HookLibraryName_x64 = "NativeLangHook_x64";
    private const string HookLibraryName_x86 = "NativeLangHook_x86";

    private const string GetLayoutChangedMessageCodeProcName = "GetLayoutChangedMessageCode";
    private const string GetLayoutChangeRequestMessageCodeProcName = "GetLayoutChangeRequestMessageCode";
    private const string SetLangHookProcName = "SetLangHook";
    
    private delegate SafeLayoutChangeHookHandle SetLangHookDelegate(IntPtr captureWindowHandler);
    private delegate uint GetMessageCodeDelegate();

    private readonly bool _is64 = IntPtr.Size == 8;

    private readonly IMessageReceiver _msgWindow;
    private readonly SafeLibraryHandle _hookLibraryHandle;
    private readonly SafeLayoutChangeHookHandle _hook;
    private readonly WrapperController? _wrapperController;
    private readonly uint _layoutChangeRequestMessageCode;
    private readonly uint _layoutChangedMessageCode;

    private KeyboardLayout _currentLayout;

    public event EventHandler<LayoutChangedEventArgs>? LayoutChanged;

    public ChangeLayoutHook(IMessageReceiver messageReceiver)
    {
        _msgWindow = messageReceiver;
        if (_msgWindow.WindowHandle == IntPtr.Zero)
            throw new ApplicationException("Can't get message window handler.");

        _msgWindow.WindowsMessageReceived += OnMessageCaptured;

        _currentLayout = KeyboardLayoutInfo.GetForegroundWindowLayout();

        var libraryName = _is64 ? HookLibraryName_x64 : HookLibraryName_x86;
        _hookLibraryHandle = DllHelper.LoadLibrary(libraryName);
        
        _layoutChangeRequestMessageCode = GetLayoutChangeRequestMessageCode(_hookLibraryHandle);
        _layoutChangedMessageCode = GetLayoutChangedMessageCode(_hookLibraryHandle);

        _hook = InitHook(_hookLibraryHandle, _msgWindow.WindowHandle);
        if (!_is64) return;

        _wrapperController = new WrapperController();
        _wrapperController.LayoutChanged += (_, hkl) => InvokeLayoutChanged(hkl);
    }
    
    public void ChangeLayoutRequest(KeyboardLayout target)
    {
        var focusWindow = GetFocusedWnd();
        var isWow64ProcessWindow = ProcessHelper.IsWow64ProcessWindow(focusWindow);

        var klIdPtr = target.IsImeLayout ? (IntPtr)target.KLID : IntPtr.Zero;
        var hklPtr = target.IsImeLayout ? IntPtr.Zero : (IntPtr)target.Hkl;

        if (isWow64ProcessWindow)
        {
            _wrapperController?.ChangeLayoutRequest(focusWindow, klIdPtr, hklPtr);
        }
        else
        {
            User32.SendMessage(focusWindow, _layoutChangeRequestMessageCode, klIdPtr, hklPtr);
        }
    }

    private void OnMessageCaptured(object? sender, WindowsMessageArgs e)
    {
        var message = e.Msg;
        var lParam = e.LParam;

        if(message != _layoutChangedMessageCode) return;
        
        uint newHkl;
        if (IntPtr.Size == 4)
        {
            newHkl = (uint)lParam;
        }
        else
        {
            var lParamHigh = (uint)(lParam.ToInt64() >> 32);
            var lParamLow = (uint)(lParam.ToInt64() & 0xFFFFFFFF);
            newHkl = lParamHigh is 0 or 0xFFFFFFFF ? lParamLow : lParamHigh;
        }
        
        InvokeLayoutChanged(newHkl);
    }

    private void InvokeLayoutChanged(uint hkl)
    {
        if (hkl == _currentLayout.Hkl) return;

        _currentLayout = KeyboardLayout.GetLayout(hkl);

        var args = new LayoutChangedEventArgs(_currentLayout);
        LayoutChanged?.Invoke(this, args);
    }

    private static uint GetLayoutChangedMessageCode(SafeLibraryHandle libHandle)
    {
        var getCaptureMessageCodeProc = DllHelper.GetDllProcDelegate<GetMessageCodeDelegate>(libHandle, 
            GetLayoutChangedMessageCodeProcName);
        
        var result = getCaptureMessageCodeProc();
        if (result == 0) 
            DllHelper.ThrowLastWin32ErrorException("Can't get LayoutChanged message code.");
        return result;
    }

    private static uint GetLayoutChangeRequestMessageCode(SafeLibraryHandle libHandle)
    {
        var getLayoutSwitchMessageCodeProc = DllHelper.GetDllProcDelegate<GetMessageCodeDelegate>(
            libHandle, GetLayoutChangeRequestMessageCodeProcName);
        
        var result = getLayoutSwitchMessageCodeProc();
        if (result == 0) 
            DllHelper.ThrowLastWin32ErrorException("Can't get LayoutChangeRequest message code.");
        
        return result;
    }

    private static SafeLayoutChangeHookHandle InitHook(SafeLibraryHandle libHandle, IntPtr captureWindowHandler)
    {
        var setHookProc = DllHelper.GetDllProcDelegate<SetLangHookDelegate>(
            libHandle, SetLangHookProcName);
        
        var result = setHookProc(captureWindowHandler);
        if (result.IsInvalid) 
            DllHelper.ThrowLastWin32ErrorException("Can't set input language hook.");

        return result;
    }

    private static IntPtr GetFocusedWnd()
    {
        var foregroundWnd = User32.GetForegroundWindow();
        var foregroundThread = User32.GetWindowThreadProcessId(foregroundWnd, out _);

        var info = new Guithreadinfo()
        {
            cbSize = Marshal.SizeOf<Guithreadinfo>(),
        };
        User32.GetGUIThreadInfo(foregroundThread, ref info);

        var focus = info.hwndCaret != IntPtr.Zero ? info.hwndCaret :info.hwndFocus;
        return focus;
    }
    
    #region Dispose

    private bool _disposed;

    ~ChangeLayoutHook()
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
            _wrapperController?.Dispose();
            _hook.Dispose();
            _hookLibraryHandle.Dispose();
        }
        //free unmanaged resources (unmanaged objects) and override finalizer
        //set large fields to null

        _disposed = true;
    }

    #endregion
}