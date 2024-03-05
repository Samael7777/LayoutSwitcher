using System.Windows;
using System.Windows.Interop;

namespace LayoutSwitcher.MessageWindow;


public partial class MsgOnlyWindow
{
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
        _hWndSource?.AddHook(WndProc);
        _initEvent.Set();
    }

    private IntPtr WndProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        var message = new MessageEventArgs(hWnd, msg, wParam, lParam);
        MessageCaptured?.Invoke(this, message);
        handled = message.Handled;
        return IntPtr.Zero;
    }
}