using System.Collections.Immutable;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using CommunityToolkit.Mvvm.ComponentModel;
using LayoutControl;
using LayoutSwitcher.JsonConverters;
using LayoutSwitcher.Models;
using VirtualKeys;
// ReSharper disable UnusedParameterInPartialMethod

namespace LayoutSwitcher;

[JsonSerializable(typeof(Settings))]
public partial class Settings : ObservableObject
{
    public const string SettingsFileName = "Settings.json";

    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        Converters =
        {
            new JsonKeyboardLayoutConverter()
        }
    };

    private readonly AppAutorun _appAutorun = new();

    [ObservableProperty]
    private ImmutableList<KeyboardLayout> _cycledLayouts = ImmutableList<KeyboardLayout>.Empty;

    [ObservableProperty] 
    private HotKey _appToggleHotkey = HotKey.Empty;

    [ObservableProperty] 
    private bool _useHookToChangeLayout;

    [JsonIgnore]
    public bool IsChanged { get; private set; }

    [JsonIgnore]
    public bool Autorun
    {
        get => _appAutorun.Autorun;
        set => _appAutorun.Autorun = value;
    }

    public async Task SaveSettingsToFileAsync()
    {
        var jsonData = JsonSerializer.Serialize(this, JsonSerializerOptions);
        await using var writer = new StreamWriter(Settings.SettingsFileName);
        await writer.WriteAsync(jsonData);
        await writer.FlushAsync();
    }

    public static Settings LoadSettingsFromFile()
    {
        if (!File.Exists(SettingsFileName)) return new Settings();
        
        using var jsonStream = File.OpenRead(SettingsFileName);
        var loaded = JsonSerializer.Deserialize<Settings>(jsonStream, JsonSerializerOptions);
        return loaded ?? new Settings();
    }

    partial void OnCycledLayoutsChanged(ImmutableList<KeyboardLayout> value)
    {
        IsChanged = true;
    }

    partial void OnAppToggleHotkeyChanged(HotKey value)
    {
        IsChanged = true;
    }

    partial void OnUseHookToChangeLayoutChanged(bool value)
    {
	    IsChanged = true;
    }
}