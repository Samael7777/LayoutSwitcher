using IpcCommunicator;
using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace LayoutControl.Wrapper;

public class WrapperController : IDisposable
{
    private const int IntSize = sizeof(int);
    private const string WrapperExeName = "NativeLangHookWrapper.exe";
    private const string WrapperAppId = "NativeLangHookWrapper";
    private const string PipeName = $"{WrapperAppId}IPC";
    private const string WrapperInitEventName = $"{WrapperAppId}InitEvent";

    private readonly ClientPipeBinary _clientPipe;
    private readonly EventWaitHandle _initWrapperEvent;

    public event EventHandler<uint>? LayoutChanged;

    public WrapperController()
    {
        _initWrapperEvent = new EventWaitHandle(false, EventResetMode.ManualReset, 
            WrapperInitEventName);

        CreateWrapperProcess();

        _clientPipe = new ClientPipeBinary(".", PipeName);
        _clientPipe.Error += OnWrapperError;
        _clientPipe.DataReceived += OnDataReceived;
        _clientPipe.Connect(5000);
    }

    public void ChangeLayoutRequest(IntPtr hWnd, IntPtr klId, IntPtr hkl)
    {
        var commandBuffer = new byte[4 * IntSize];
        BitConverter.GetBytes((int)WrapperCommand.ChangeLayout).CopyTo(commandBuffer, 0);
        BitConverter.GetBytes((uint)hWnd).CopyTo(commandBuffer, IntSize);
        BitConverter.GetBytes((uint)klId).CopyTo(commandBuffer, IntSize * 2);
        BitConverter.GetBytes((uint)hkl).CopyTo(commandBuffer, IntSize * 3);
        
        _clientPipe.Send(commandBuffer);
    }

    private void OnDataReceived(object? sender, PipeBinaryDataReceivedEventArgs e)
    {
        var wrapperMessageCode = (WrapperMessage)BitConverter.ToInt32(e.Data);
        switch (wrapperMessageCode)
        {
            case WrapperMessage.LayoutChanged:
                var hkl = BitConverter.ToUInt32(e.Data, IntSize);
                LayoutChanged?.Invoke(this, hkl);
                break;
            case WrapperMessage.Error:
                ThrowWrapperError(e.Data[IntSize..]);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private static void ThrowWrapperError(byte[] data)
    {
        var json = Encoding.UTF8.GetString(data);
        var exception = JsonSerializer.Deserialize<Exception>(json);
        if (exception != null)
            throw new ApplicationException("Wrapper exception.", innerException: exception);
    }

    private void OnWrapperError(object? sender, PipeErrorEventArgs e)
    {
        throw new ApplicationException("Wrapper communication error.", innerException: e.Exception);
    }
    
    private void CreateWrapperProcess()
    {
        _initWrapperEvent.Reset();
        var wrapperProcessStartInfo = new ProcessStartInfo
        {
            FileName = WrapperExeName,
            CreateNoWindow = true,
            UseShellExecute = false,
            RedirectStandardOutput = true,
        };
        Process.Start(wrapperProcessStartInfo);

        try
        {
            _initWrapperEvent.WaitOne(5000, false);
        }
        catch (TimeoutException)
        {
            throw new ApplicationException("Can't start hook wrapper process");
        }
    }

    private void SendExitCommand()
    {
        var commandBuffer = BitConverter.GetBytes((int)WrapperCommand.Exit);
        _clientPipe.Send(commandBuffer);
    }

    #region Dispose

    private bool _disposed;

    ~WrapperController()
    {
        Dispose(false);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;
        if (disposing)
        {
            //dispose managed state (managed objects)
            SendExitCommand();
            _clientPipe.Dispose();
            _initWrapperEvent.Dispose();
        }
        //free unmanaged resources (unmanaged objects) and override finalizer
        //set large fields to null

        _disposed = true;
    }

    #endregion
}