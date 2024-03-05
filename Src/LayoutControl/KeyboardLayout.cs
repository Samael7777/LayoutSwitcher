using System.Diagnostics;
using System.Globalization;
using LayoutControl.Tools;
// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo

namespace LayoutControl;

public enum LayoutType
{
    Unknown,
    Standart,
    MSKLC,
    TSF,
    IME
}

[DebuggerDisplay("{Hkl} ({LanguageDisplayName})")]
public class KeyboardLayout : IEquatable<KeyboardLayout>
{
    private static readonly Dictionary<uint, KeyboardLayout> Cache = new();

    private KeyboardLayout(uint hkl)
    {
        Hkl = hkl;
        LanguageId = (ushort)(hkl & 0xFFFF);
        var layoutId = (ushort)(hkl >> 16);
        KeyboardId = layoutId;
        Type = GetType(hkl);
        var descriptor = LayoutIdentifier.GetKeyboardLayout(layoutId);
        KLID = (uint)descriptor.KLID;
        LanguageDisplayName = CultureInfo.GetCultureInfo(LanguageId).DisplayName;
        LayoutDisplayName = descriptor.LayoutDisplayName;
        LayoutText = descriptor.LayoutText;
    }

    public LayoutType Type { get; }

    /// <summary>
    /// Input locale identifier.
    /// </summary>
    public uint Hkl { get; }
    
    /// <summary>
    /// Layout identifier.
    /// </summary>
    public uint KLID { get; }

    /// <summary>
    /// Language identifier.
    /// </summary>
    public ushort LanguageId { get; }

    /// <summary>
    /// Keyboard layout identifier.
    /// </summary>
    public uint KeyboardId { get; }

    /// <summary>
    /// Language name, localized for default system language.
    /// </summary>
    public string LanguageDisplayName { get; }

    /// <summary>
    /// Keyboard layout name, localized for default system language.
    /// </summary>
    public string LayoutDisplayName { get; }

    /// <summary>
    /// Keyboard layout text from system registry.
    /// </summary>
    public string LayoutText { get; }

    public bool IsImeLayout => (KeyboardId & 0xF000) > 0;

    public override string ToString() => Hkl.ToString("x8");

    public static KeyboardLayout GetLayout(uint hkl)
    {
        if (hkl == 0)
            throw new ArgumentOutOfRangeException(nameof(hkl));

        if(Cache.TryGetValue(hkl, out var layout))
            return layout;

        layout = new KeyboardLayout(hkl);
        Cache.Add(hkl, layout);
        
        return layout;
    }

    private static LayoutType GetType(uint hkl)
    {
        return (hkl & 0xF0000000) switch
        {
            0x0 => LayoutType.Standart,
            0xA => LayoutType.MSKLC,
            0xD => LayoutType.TSF,
            0xF => LayoutType.IME,
            _ => LayoutType.Unknown
        };
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