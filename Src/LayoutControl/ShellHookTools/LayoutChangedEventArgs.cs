namespace LayoutControl.ShellHookTools;

public class LayoutChangedEventArgs : EventArgs
{
    public LayoutChangedEventArgs(KeyboardLayout newLayout)
    {
        NewLayout = newLayout;
    }
    public KeyboardLayout NewLayout { get; }
}