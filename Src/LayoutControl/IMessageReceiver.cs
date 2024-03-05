namespace LayoutControl;

public interface IMessageReceiver
{
    public event EventHandler<WindowsMessageArgs> WindowsMessageReceived;
    public IntPtr WindowHandle { get; }
}