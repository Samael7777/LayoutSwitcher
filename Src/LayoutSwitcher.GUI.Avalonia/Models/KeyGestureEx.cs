using System.Text;
using Avalonia.Input;
using Avalonia.Input.Platform;

namespace LayoutSwitcher.GUI.Avalonia.Models;

internal static class KeyGestureEx
{

    /// <summary>
    /// Based on <c>Avalonia.Input.KeyGesture.ToString()</c>
    /// </summary>
    public static string GetGestureString(this KeyGesture gesture)
    {
        var formatInfo = KeyGestureFormatInfo.Invariant;
        var s = new StringBuilder();

        if (gesture.KeyModifiers.HasFlag(KeyModifiers.Control))
        {
            s.Append(formatInfo.Ctrl);
        }

        if (gesture.KeyModifiers.HasFlag(KeyModifiers.Shift))
        {
            Plus(s);
            s.Append(formatInfo.Shift);
        }

        if (gesture.KeyModifiers.HasFlag(KeyModifiers.Alt))
        {
            Plus(s);
            s.Append(formatInfo.Alt);
        }

        if (gesture.KeyModifiers.HasFlag(KeyModifiers.Meta))
        {
            Plus(s);
            s.Append(formatInfo.Meta);
        }

        if ((gesture.Key != Key.None) || (gesture.KeyModifiers == KeyModifiers.None))
        {
            Plus(s);
            s.Append(formatInfo.FormatKey(gesture.Key));
        }
        
        return s.ToString();

        static void Plus(StringBuilder s)
        {
            if (s.Length > 0)
            {
                s.Append('+');
            }
        }
    }
}