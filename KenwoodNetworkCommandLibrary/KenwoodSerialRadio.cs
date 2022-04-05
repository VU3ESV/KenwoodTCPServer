global using System.IO.Ports;

namespace Kenwood;

public class KenwoodSerialRadio : IKenwoodRadio, IDisposable
{
#pragma warning disable CA1416 // Validate platform compatibility

    private readonly RadioPort _radioPort;
    private SerialPort _serialPort;
    private readonly RadioType _radioType;
    private volatile bool _isConnected;

    private KenwoodSerialRadio(RadioPort radioPort,
                               RadioType radioType)
    {
        _radioPort = radioPort;
        _radioType = radioType;
    }

    public static KenwoodSerialRadio Create(RadioPort radioPort,
                                            RadioType radioType)
    {
        KenwoodSerialRadio kenwoodSerialRadio = new(radioPort, radioType);
        return kenwoodSerialRadio;
    }

    public RadioType RadioType => _radioType;

    public bool IsConnected => _isConnected;

    public Task<bool> ConnectAsync(CancellationToken cancellationToken)
    {
        if (_isConnected)
        {
            return Task.FromResult(_isConnected);
        }

        _serialPort = new(_radioPort.Comport,
                          _radioPort.BaudRate,
                          _radioPort.Parity,
                          _radioPort.DataBits,
                          _radioPort.StopBits)
        {
            ReadTimeout = _radioPort.ReadTimeout,
            WriteTimeout = _radioPort.WriteTimeout,
            Handshake = _radioPort.Handshake,
            DtrEnable = true,
            RtsEnable = true
        };

        _serialPort.ErrorReceived += SerialPort_ErrorReceived;

        if (_serialPort.IsOpen)
        {
            return Task.FromResult(_isConnected);
        }

        _serialPort.Open();
        if (_serialPort.IsOpen)
        {
            _isConnected = true;
        }
        else
        {
            _isConnected = false;
        }

        return Task.FromResult(_isConnected);
    }

    private void SerialPort_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
    {
        switch (e.EventType)
        {
            case SerialError.RXOver:
            case SerialError.Overrun:
            case SerialError.RXParity:
            case SerialError.Frame:
            case SerialError.TXFull:
                _serialPort.ReadExisting();
                _serialPort.Close();
                _isConnected = false;
                break;
        }
    }

    public Task DisconnectAsync()
    {
        _serialPort.ErrorReceived -= SerialPort_ErrorReceived;

        if (_serialPort.IsOpen)
        {
            _serialPort.Close();
            _isConnected = false;
        }
        else
        {
            _isConnected = false;
        }

        return Task.CompletedTask;
    }

    public Task<string> SendAsync(string command, CancellationToken cancellationToken)
    {
        string response = string.Empty;
        if (cancellationToken.IsCancellationRequested)
        {
            return Task.FromResult(string.Empty);
        }

        if (_serialPort.IsOpen)
        {
            _serialPort.Write(command);
            response = _serialPort.ReadExisting();
        }

        return Task.FromResult(response);
    }

    public async Task<List<string>> SendAsync(List<string> commands, CancellationToken cancellationToken)
    {
        var responses = new List<string>();
        foreach (var command in commands)
        {
            var response = await SendAsync(command, cancellationToken).ConfigureAwait(false);
            responses.Add(response);
        }

        return responses;
    }

    public void Dispose()
    {
        if (_serialPort.IsOpen)
        {
            GC.SuppressFinalize(this);
            _serialPort.Close();
            _serialPort.Dispose();
        }
    }
#pragma warning restore CA1416 // Validate platform compatibility
}
