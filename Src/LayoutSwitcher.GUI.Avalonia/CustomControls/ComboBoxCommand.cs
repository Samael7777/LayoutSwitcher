using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;

namespace LayoutSwitcher.GUI.Avalonia.CustomControls;

public class ComboBoxCommand : AvaloniaObject
{
   public static readonly AttachedProperty<ICommand> DropDownClosedCommandProperty =
        AvaloniaProperty.RegisterAttached<ComboBoxCommand, ComboBox, ICommand>(
            "DropDownClosedCommand", 
            defaultBindingMode:BindingMode.OneTime);

    public static void SetDropDownClosedCommand(ComboBox cb, ICommand value)
    {
        cb.SetValue(DropDownClosedCommandProperty, value);
    }

    public static ICommand GetDropDownClosedCommand(ComboBox cb)
    {
        return cb.GetValue(DropDownClosedCommandProperty);
    }
}