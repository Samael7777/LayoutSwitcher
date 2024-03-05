namespace LayoutSwitcher.MessageWindow;

public class MessageEventArgs : EventArgs
{
    public MessageEventArgs(IntPtr HWnd, int Msg, IntPtr WParam, IntPtr LParam, bool Handled = false)
    {
        this.HWnd = HWnd;
        this.Msg = Msg;
        this.WParam = WParam;
        this.LParam = LParam;
        this.Handled = Handled;
    }

    public IntPtr HWnd { get; init; }
    public int Msg { get; init; }
    public IntPtr WParam { get; init; }
    public IntPtr LParam { get; init; }
    public bool Handled { get; init; }
}