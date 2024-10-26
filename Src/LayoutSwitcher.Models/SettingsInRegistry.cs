using LayoutSwitcher.Control;
using Microsoft.Win32;

namespace LayoutSwitcher.Models;

public class SettingsInRegistry : BaseSettings
{
    private const string LayoutToggleHotKeyIndexValueName = "LayoutToggleHotKeyIndex";
    private const string CycledLayoutValueName = "CycledLayout";
    
    private readonly string _appSubKey;

    public SettingsInRegistry(string appKey)
    {
        _appSubKey = "Software\\" + appKey;
    }

    public override void Load()
    {
        var appSubKey = Registry.CurrentUser.OpenSubKey(_appSubKey);
        if (appSubKey == null) return;

        layoutToggleHotKeyIndex = appSubKey.GetValue(LayoutToggleHotKeyIndexValueName) as int?
                                  ?? layoutToggleHotKeyIndex;

        var cycledLayoutData = appSubKey.GetValue(CycledLayoutValueName) as byte[]
                               ?? Array.Empty<byte>();

        cycledLayout = BytesToUIntList(cycledLayoutData);
    }

    public override void Save()
    {
        if (!isChanged) return;

        var cycledLayoutBinaryData = UIntArrayToBytes(cycledLayout);

        var appSubKey = Registry.CurrentUser.OpenSubKey(_appSubKey, true) 
                        ?? Registry.CurrentUser.CreateSubKey(_appSubKey);

        appSubKey.SetValue(LayoutToggleHotKeyIndexValueName, LayoutToggleHotKeyIndex, RegistryValueKind.DWord);
        appSubKey.SetValue(CycledLayoutValueName, cycledLayoutBinaryData, RegistryValueKind.Binary);
    }

    private static byte[] UIntArrayToBytes(IEnumerable<KeyboardLayout> data)
    {
        using var stream = new MemoryStream();
        using var writer = new BinaryWriter(stream);
        foreach (var item in data)
        {
            writer.Write(item.Hkl);
        }

        return stream.ToArray();
    }

    private static IEnumerable<KeyboardLayout> BytesToUIntList(byte[] buffer)
    {
        if (buffer.Length == 0) return Array.Empty<KeyboardLayout>();

        var data = new List<KeyboardLayout>(buffer.Length / sizeof(uint));

        using var stream = new MemoryStream(buffer);
        using var reader = new BinaryReader(stream);
        while (stream.Position < buffer.Length)
        {
            var hkl = reader.ReadUInt32();
            var layout = KeyboardLayout.GetLayout(hkl);
            data.Add(layout);
        }

        return data;
    } 
}