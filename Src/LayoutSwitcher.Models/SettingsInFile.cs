using LayoutSwitcher.Control;

namespace LayoutSwitcher.Models;

public class SettingsInFile : BaseSettings
{
    private readonly string _settingsPath;

    
    public SettingsInFile(string settingsPath)
    {
        _settingsPath = settingsPath;
    }
    
    public override void Save()
    {
        if (!isChanged) return;
        
        using var file = File.Create(_settingsPath);
        using var writer = new BinaryWriter(file);
        
        writer.Write(LayoutToggleHotKeyIndex);

        foreach (var layout in CycledLayout)
        {
            writer.Write(layout.Hkl);
        }
    }

    public override void Load()
    {
        if (!File.Exists(_settingsPath)) return;

        var layoutsList = new List<KeyboardLayout>();

        using var file = File.OpenRead(_settingsPath);
        using var reader = new BinaryReader(file);
        try
        {
            var hotKeyIndex = reader.ReadInt32();
            while (file.Position < file.Length)
            {
                var hkl = reader.ReadUInt32();
                var layout = KeyboardLayout.GetLayout(hkl);
                layoutsList.Add(layout);
            }

            LayoutToggleHotKeyIndex = hotKeyIndex;
            cycledLayout = layoutsList;
        }
        catch
        {
            // ignored
        }
    }
}