using System.Globalization;
using Microsoft.Win32;

namespace LayoutSwitcher.Control.Tools;

// ReSharper disable once InconsistentNaming
// ReSharper disable once IdentifierTypo
internal record LayoutDescriptor(int KLID, string DisplayNameResource,
    string LayoutDisplayName, string LayoutText);

internal static class LayoutIdentifier
{
    private record StringResource(string LibFilename, uint ResourceId);


    private static readonly Dictionary<ushort, LayoutDescriptor> s_Cache;
    private static readonly Dictionary<string, DllResourceReader> s_ResourceReaders;

    static LayoutIdentifier()
    {
        s_Cache = new Dictionary<ushort, LayoutDescriptor>();
        s_ResourceReaders = new Dictionary<string, DllResourceReader>();
        try { InitCache(); }
        finally { DisposeResourceReaders(); }
    }

    public static LayoutDescriptor GetKeyboardLayout(ushort keyboardId)
    {
        s_Cache.TryGetValue((ushort)(keyboardId & 0x0FFF), out var descriptor);
        
        return descriptor 
               ?? throw new ApplicationException($"System doesn't contain layout 0x{keyboardId:X8}");
    }

    private static void InitCache()
    {
        var layoutsRegKey = GetLayoutRegistryKey();

        foreach (var layoutKeyName in layoutsRegKey.GetSubKeyNames())
        {
            var layoutRegKey = layoutsRegKey.OpenSubKey(layoutKeyName)
                               ?? throw new ApplicationException("Error accessing system registry.");

            if (!int.TryParse(layoutKeyName, NumberStyles.HexNumber, CultureInfo.InvariantCulture,
                    out var klId))
                continue;

            var layoutKey = GetLayoutKey(klId, layoutRegKey);

            var displayNameResource = layoutRegKey.GetValue("Layout Display Name") as string ?? "";
            var layoutText = layoutRegKey.GetValue("Layout Text") as string ?? "";

            var layoutDisplayName = GetLayoutDisplayName(displayNameResource);
            if (string.IsNullOrWhiteSpace(layoutDisplayName))
                layoutDisplayName = $"Unknown keyboard layout id: {klId}";


            var layoutDescriptor = new LayoutDescriptor(klId, displayNameResource,
                layoutDisplayName, layoutText);

            s_Cache.TryAdd((ushort)layoutKey, layoutDescriptor);
        }
    }

    private static RegistryKey GetLayoutRegistryKey()
    {
        return Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\Keyboard Layouts")
               ?? throw new ApplicationException("Error accessing system registry.");
    }

    private static int GetLayoutKey(int layoutId, RegistryKey layoutRegKey)
    {
        var layoutKeyString = layoutRegKey.GetValue("Layout Id") as string;
        return string.IsNullOrWhiteSpace(layoutKeyString)
            ? layoutId
            : int.Parse(layoutKeyString, NumberStyles.HexNumber);
    }

    private static string GetLayoutDisplayName(string resourceString)
    {
        if (string.IsNullOrWhiteSpace(resourceString)) return string.Empty;

        var (libFilename, resourceId) = ParseResourcePath(resourceString);
        var libName = Path.GetFileName(libFilename).ToLower();

        if (!s_ResourceReaders.TryGetValue(libName, out var reader))
        {
            reader = new DllResourceReader(libFilename);
            s_ResourceReaders.Add(libName, reader);
        }
        
        return reader.ReadStringResource(resourceId);
    }

    private static StringResource ParseResourcePath(string resourcePath)
    {
        if (string.IsNullOrWhiteSpace(resourcePath))
            throw new ArgumentNullException(nameof(resourcePath));

        var parts = resourcePath.Split(',');
        if (parts.Length != 2)
            throw new ArgumentException($"Resource string {resourcePath} is invalid.");

        var pathString = Environment.ExpandEnvironmentVariables(parts[0].TrimStart('@'));
        var idString = parts[1].TrimStart('-');

        return new StringResource(pathString, uint.Parse(idString));
    }

    private static void DisposeResourceReaders()
    {
        foreach (var reader in s_ResourceReaders.Values)
        {
            reader.Dispose();
        }
        s_ResourceReaders.Clear();
    }
}