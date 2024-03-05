﻿using System.Collections.Immutable;
using LayoutControl.Extensions;
using Microsoft.Win32;
using VirtualKeys;

namespace LayoutControl;

public enum ToggleHotKey
{
    AltShift = 1,
    CtrShift = 2,
    None = 3,
}

public static class LayoutsHotkeys
{
    public static IReadOnlyDictionary<uint, HotKey> GetLayoutDirectSwitchHotkeys()
    {
        var hotkeys = new Dictionary<uint, HotKey>();
        var hcu = Registry.CurrentUser;
        var hotKeysList = hcu.OpenSubKey(@"Control Panel\Input Method\Hot Keys\")!;

        foreach (var item in hotKeysList.GetSubKeyNames())
        {
            var data = hotKeysList.OpenSubKey(item);

            if (data?.GetValue("Target IME") is not byte[] targetImeBytes)
                continue;               //Broken data
            var targetIme = BitConverter.ToUInt32(targetImeBytes);

            if (data.GetValue("Key Modifiers") is not byte[] modifiersBytes)
                continue;               //Broken data
            var modifiers = BitConverter.ToInt32(modifiersBytes) & 0xFF;

            if (data.GetValue("Virtual Key") is not byte[] virtualKeyBytes)
                continue;               //Broken data
            var virtualKeyCode =  BitConverter.ToInt32(virtualKeyBytes);

            var hotkey = new HotKey((Modifiers)modifiers, (Key)virtualKeyCode);
            hotkeys.AddOrUpdate(targetIme, hotkey, (_, _) => hotkey);
        }

        return hotkeys.ToImmutableDictionary();
    }
    
    public static ToggleHotKey GetLayoutToggleHotkey()
    {
        var regKey = Registry.CurrentUser.OpenSubKey(@"Keyboard Layout\Toggle")
                     ?? throw new ApplicationException("Error accessing system registry.");

        var hotkeysValue = regKey.GetValue("Language Hotkey") as string;
        if (string.IsNullOrWhiteSpace(hotkeysValue)) return ToggleHotKey.None;

        return int.TryParse(hotkeysValue, out var value) 
            ? Enum.IsDefined(typeof(ToggleHotKey), value) ? (ToggleHotKey)value : ToggleHotKey.None
            : ToggleHotKey.None;
    }
}