using System.Globalization;
using LayoutControl.PInvoke;
using Microsoft.Win32;
// ReSharper disable IdentifierTypo
// ReSharper disable InconsistentNaming

namespace LayoutControl.Tools;

internal record LayoutDescriptor(int KLID, string DisplayNameResource,
    string LayoutDisplayName, string LayoutText);

internal static class LayoutIdentifier
{
    private record StringResource(string LibFilename, int ResourceId);


    private static readonly Dictionary<ushort, LayoutDescriptor> Cache;
    private static readonly Dictionary<string, DllResourceReader> ResourceReaders;

    static LayoutIdentifier()
    {
        Cache = new Dictionary<ushort, LayoutDescriptor>();
        ResourceReaders = new Dictionary<string, DllResourceReader>();
        InitCache();
        DisposeResourceReaders();
    }

    public static LayoutDescriptor GetKeyboardLayout(ushort keyboardId)
    {
        Cache.TryGetValue((ushort)(keyboardId & 0x0FFF), out var descriptor);
        if (descriptor == null)
            throw new ApplicationException($"System doesn't contain layout 0x{keyboardId:X8}");

        return descriptor;
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

            Cache.TryAdd((ushort)layoutKey, layoutDescriptor);
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
        if (ResourceReaders.TryGetValue(libName, out var reader))
            return reader.ReadStringResource(resourceId);

        reader = new DllResourceReader(libFilename);
        ResourceReaders.Add(libName, reader);

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

        return new StringResource(pathString, int.Parse(idString));
    }

    private static void DisposeResourceReaders()
    {
        foreach (var reader in ResourceReaders.Values)
        {
            reader.Dispose();
        }
        ResourceReaders.Clear();
    }
}