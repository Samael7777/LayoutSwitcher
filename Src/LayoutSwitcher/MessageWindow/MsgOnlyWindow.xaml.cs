using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace LayoutSwitcher.MessageWindow;


public partial class MsgOnlyWindow
{
    private const int HwndMessage = -3;
    private readonly ManualResetEventSlim _initEvent;

    private HwndSource? _hWndSource;

    public event EventHandler<MessageEventArgs>? MessageCaptured;
    
    public MsgOnlyWindow()
    {
        _initEvent = new ManualResetEventSlim(false);
        InitializeComponent();
        
        Visibility = Visibility.Collapsed;
        ShowInTaskbar = false;
        MaxHeight = 0;
        MaxWidth = 0;
        ResizeMode = ResizeMode.NoResize;
        WindowStartupLocation = WindowStartupLocation.Manual;
        WindowStyle = WindowStyle.None;

        _hWndSource = PresentationSource.FromVisual(this) as HwndSource;
        _hWndSource?.AddHook(WndProc);
        Show();
    }

    public IntPtr WndHandle
    {
        get
        {
            _initEvent.Wait();
            return _hWndSource?.Handle
                   ?? throw new ApplicationException("Error creating window");
        }
    }

    public void Invoke(Action action)
    {
        _initEvent.Wait();
        _hWndSource?.Dispatcher.Invoke(action);
    }

    protected override void OnSourceInitialized(EventArgs e)
    {
        base.OnSourceInitialized(e);
        _hWndSource = PresentationSource.FromVisual(this) as HwndSource;
        if (_hWndSource == null) 
            throw new ApplicationException("Error initializing message window.");

        _hWndSource.AddHook(WndProc);
        var oldParent = SetParent(_hWndSource.Handle, (IntPtr)HwndMessage);
        if (oldParent == IntPtr.Zero)
        {
			var error = Marshal.GetLastWin32Error();
            throw new ApplicationException($"Error initializing message window. Error : 0x{error:x8}");
        }
           
        _initEvent.Set();
    }

    private IntPtr WndProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        var message = new MessageEventArgs(hWnd, msg, wParam, lParam);
        MessageCaptured?.Invoke(this, message);
        handled = message.Handled;
        return IntPtr.Zero;
    }

    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);
}