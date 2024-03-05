using System.ComponentModel;
using System.Runtime.InteropServices;

namespace LayoutControl.PInvoke;

internal static class DllHelper
{
    public static SafeLibraryHandle LoadLibrary(string dllName)
    {
        var handle = Kernel32.LoadLibrary(dllName);
        if(handle.IsInvalid) ThrowLastWin32ErrorException($"Error loading library {dllName}.");
        return handle;
    }

    public static TDelegate GetDllProcDelegate<TDelegate>(SafeLibraryHandle libraryHandle, string procName)
    {
        var handle = Kernel32.GetProcAddress(libraryHandle, procName);
        if(handle == IntPtr.Zero) ThrowLastWin32ErrorException($"Can't get {procName} handle.");

        return Marshal.GetDelegateForFunctionPointer<TDelegate>(handle);
    }

    public static void ThrowLastWin32ErrorException(string message)
    {
        throw new Win32Exception($"{message} Error: " + $"{Marshal.GetLastWin32Error():X8}");
    }
}