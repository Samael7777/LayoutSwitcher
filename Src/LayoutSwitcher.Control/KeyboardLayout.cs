using LayoutSwitcher.Control.Json;
using LayoutSwitcher.Control.Tools;
using System.Diagnostics;
using System.Globalization;
using System.Text.Json.Serialization;

// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo

namespace LayoutSwitcher.Control;

[JsonConverter(typeof(KeyboardLayoutJsonConverter))]
[DebuggerDisplay("{Hkl} ({LanguageDisplayName})")]
public class KeyboardLayout : IEquatable<KeyboardLayout>
{
    public static KeyboardLayout Empty { get; } = new();
    public bool IsEmpty => Hkl == 0;

    private KeyboardLayout()
    {
        Hkl = 0;
        LanguageId = 0;
        KeyboardId = 0;
        LanguageDisplayName = string.Empty;
        LayoutDisplayName = string.Empty;
        LayoutText = string.Empty;
    }

    private KeyboardLayout(uint hkl)
    {
        Hkl = hkl;
        LanguageId = (ushort)(hkl & 0xFFFF);
        var layoutId = (ushort)(hkl >> 16);
        KeyboardId = layoutId;
        LanguageDisplayName = CultureInfo.GetCultureInfo(LanguageId).DisplayName;
        var descriptor = LayoutIdentifier.GetKeyboardLayout(layoutId);
        KLID = (uint)descriptor.KLID;
        LayoutDisplayName = descriptor.LayoutDisplayName;
        LayoutText = descriptor.LayoutText;
    }
    
    /// <summary>
    /// Input locale identifier.
    /// </summary>
    [JsonInclude] public uint Hkl { get; }
    
    /// <summary>
    /// Layout identifier.
    /// </summary>
    [JsonIgnore] public uint KLID { get; }

    /// <summary>
    /// Language identifier.
    /// </summary>
    [JsonIgnore] public ushort LanguageId { get; }

    /// <summary>
    /// Keyboard layout identifier.
    /// </summary>
    [JsonIgnore] public uint KeyboardId { get; }

    /// <summary>
    /// Language name, localized for default system language.
    /// </summary>
    [JsonIgnore] public string LanguageDisplayName { get; }

    /// <summary>
    /// Keyboard layout name, localized for default system language.
    /// </summary>
    [JsonIgnore]  public string LayoutDisplayName { get; }

    /// <summary>
    /// Keyboard layout text from system registry.
    /// </summary>
    [JsonIgnore] public string LayoutText { get; }
    
    public override string ToString() => Hkl.ToString("X8");

    public static KeyboardLayout GetLayout(uint hkl)
    {
        return hkl == 0 ? Empty : new KeyboardLayout(hkl);
    }
    
    #region IEquatable
    public bool Equals(KeyboardLayout? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Hkl == other.Hkl;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((KeyboardLayout)obj);
    }

    public static bool operator ==(KeyboardLayout? left, KeyboardLayout? right) 
        => left?.Equals(right) ?? false;

    public static bool operator !=(KeyboardLayout? left, KeyboardLayout? right) 
        => !(left == right);

    public override int GetHashCode()
    {
        return (int)Hkl;
    }
    
    #endregion
}