global using System;
global using System.Collections.Generic;
global using System.Net;
global using System.Net.Sockets;
global using System.Text;
global using System.Threading;
global using System.Threading.Tasks;

namespace Kenwood;

public class KenwoodRadio : IKenwoodRadio, IDisposable
{
    private volatile Socket _clientSocket;
    private static readonly Encoding _encoding = Encoding.Unicode;
    private readonly string _userName;
    private readonly string _password;
    private readonly IPAddress _radioAddress;
    private readonly int _port;
    private readonly RadioType _radioType;
    private volatile bool _isConnected;
    private readonly IPEndPoint _ipEndpoint;
    private readonly bool _isAdmin = true;
    private const string ConnectCommand = "##CN;";
    private const string Ts990IdCommand = "##ID{0}{1}{2}{3};";
    private const string Ts890IdCommand = "##ID{0}{1}{2}{3}{4};";
    private const string PsCommand = "PS;";
    private const string AI2SetCommand = "AI2;";
    private const string AIGetCommand = "AI;";
    private readonly Timer _heartBeatTimer;
    private volatile bool _isAICommandEnabled;
    private volatile bool _isConnecting = false;

    /// <summary>
    /// Crete a new instance of the KenwoodRadio
    /// </summary>
    /// <param name="radioAddress">IP Address of the Radio</param>
    /// <param name="port">Port Number of the Radio, 60000 for the TS990</param>
    /// <param name="userName">UserName configured in ARCP990</param>
    /// <param name="password">Password configured in ARCP990</param>
    /// <param name="radioType">Radio type</param>
    private KenwoodRadio(string radioAddress,
                         int port,
                         string userName,
                         string password,
                         RadioType radioType)
    {
        _radioAddress = IPAddress.Parse(radioAddress);
        _port = port;
        _userName = userName;
        _password = password;
        _radioType = radioType;
        _ipEndpoint = new IPEndPoint(_radioAddress, _port == 0 ? 60000 : _port);
        _clientSocket = new Socket(AddressFamily.InterNetwork,
                                   SocketType.Stream,
                                   ProtocolType.Tcp);
        _clientSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
        _clientSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
        _heartBeatTimer = new Timer(new TimerCallback(CheckHeartBeat), this, 5000, 5000);
        /*If there are no communications for 10 seconds, the TCP /IP connection with the transceiver is terminated.
         To avoid this the PS Command will be sent to Radio on every 5 seconds*/
    }

    /// <summary>
    /// Crete a new instance of the KenwoodRadio
    /// </summary>
    /// <param name="radioAddress">IP Address of the Radio</param>
    /// <param name="port">Port Number of the Radio, 60000 for the TS990</param>
    /// <param name="userName">UserName configured in ARCP990</param>
    /// <param name="password">Password configured in ARCP990</param>
    /// <param name="radioType">Radio type</param>
    /// <returns> An instance of the KENWOOD Radio</returns>
    public static KenwoodRadio Create(string radioAddress,
                                      int port,
                                      string userName,
                                      string password,
                                      RadioType radioType)
    {
        KenwoodRadio kenwoodRadio = new(radioAddress,
                                        port,
                                        userName,
                                        password,
                                        radioType);
        return kenwoodRadio;
    }

    public RadioType RadioType => _radioType;

    public bool IsConnected => _isConnected;

    /// <summary>
    /// Connects to the Radio
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<bool> ConnectAsync(CancellationToken cancellationToken)
    {
        if (_clientSocket == null)
        {
            _clientSocket = new Socket(AddressFamily.InterNetwork,
                                       SocketType.Stream,
                                       ProtocolType.Tcp);
            _clientSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
            _clientSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
        }
        _isConnecting = true;
        while (!_clientSocket.Connected)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _isConnected = false;
                return false;
            }

            try
            {
                await _clientSocket.ConnectAsync(_ipEndpoint);
            }
            catch (SocketException)
            {
                //ignored
            }
        }

        var connectResponse = await SendAsync(ConnectCommand, cancellationToken);
        if (!ValidateCNResponse(connectResponse))
        {
            _isConnected = false;
            _isConnecting = false;
            return false;
        }

        var idCommand = _radioType == RadioType.TS990 ? string.Format(Ts990IdCommand,
                                                                      _userName.Length,
                                                                      _password.Length,
                                                                      _userName,
                                                                      _password)
                                                       : string.Format(Ts890IdCommand,
                                                                       _isAdmin ? "0" : "1",
                                                                       Convert.ToChar(_userName.Length),
                                                                       Convert.ToChar(_password.Length),
                                                                       _userName,
                                                                       _password);

        var idResponse = await SendAsync(idCommand, cancellationToken);

        if (!ValidateIdResponse(idResponse))
        {
            _isConnected = false;
            _isConnecting = false;
            return false;
        }

        _isAICommandEnabled = true;
        var aiGetResponse = await SendAsync(AIGetCommand, cancellationToken);
        if (!ValidateAIGetResponse(aiGetResponse))
        {
            var aiSetResponse = await SendAsync(AI2SetCommand, cancellationToken).ConfigureAwait(false);
            if (!ValidateAISetResponse(aiSetResponse))
            {
                _isConnected = false;
                _isConnecting = false;
                return false;
            }
        }

        _isConnecting = false;
        _isConnected = true;
        return true;
    }

    public Task DisconnectAsync()
    {
        if (_clientSocket == null || _clientSocket.Connected == false)
        {
            _isConnected = false;
            return Task.CompletedTask;
        }

        var disconnected = _clientSocket.DisconnectAsync(new SocketAsyncEventArgs());
        if (disconnected)
        {
            _clientSocket.Shutdown(SocketShutdown.Both);
            _clientSocket.Close();
        }

        _isConnected = false;
        return Task.CompletedTask;
    }

    public async Task<List<string>> SendAsync(List<string> commands, CancellationToken cancellationToken)
    {
        var receivePayLoad = new List<string>();
        if (_clientSocket == null || _clientSocket.Connected == false)
        {
            return receivePayLoad;
        }

        foreach (var command in commands)
        {
            var receivedData = await SendAsync(command, cancellationToken).ConfigureAwait(false);
            receivePayLoad.Add(receivedData);
        }

        return receivePayLoad;
    }

    public async Task<string> SendAsync(string command, CancellationToken cancellationToken)
    {
        try
        {
            var receivePayLoad = string.Empty;
            if (_clientSocket == null || _clientSocket.Connected == false)
            {
                return receivePayLoad;
            }

            byte[] sendBuffer = _encoding.GetBytes(command);
            ArraySegment<byte> sendBufferClone = new(sendBuffer);
            ArraySegment<byte> receiveBufferClone = new(new byte[1024]);
            try
            {
                var sendLength = await _clientSocket.SendAsync(sendBufferClone, SocketFlags.None).ConfigureAwait(false);
                if (sendLength == 0)
                {
                    // Unable to send the data to radio, may be a network Error or Radio is Off :)
                    return receivePayLoad;
                }
            }
            catch (SocketException)
            {
                Close(_clientSocket);
                // _isConnected = false;
                return receivePayLoad;
            }

            try
            {
                var receiveLength = await _clientSocket.ReceiveAsync(receiveBufferClone, SocketFlags.None).ConfigureAwait(false);
                byte[] data = new byte[receiveLength];
                Array.Copy(receiveBufferClone.Array, data, receiveLength);
                var receivedData = _encoding.GetString(data);
                return receivedData;
            }
            catch (SocketException)
            {
                Close(_clientSocket);
                // _isConnected = false;
                return receivePayLoad;
            }
        }
        catch (Exception)
        {
            return string.Empty;
        }
    }
    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _clientSocket?.Shutdown(SocketShutdown.Both);
        _clientSocket?.Close();
        _clientSocket?.Dispose();
        _heartBeatTimer.Dispose();
    }

    private static void Close(Socket socket)
    {
        //socket.Dispose();
        //socket.Disconnect(true);
    }

    private static bool ValidateCNResponse(string cnResponse)
    {
        return cnResponse.Contains("##CN1");
    }

    private static bool ValidateIdResponse(string idResponse)
    {
        return idResponse.Contains("##ID1");
    }

    private static bool ValidateAIGetResponse(string aiResponse)
    {
        return aiResponse.Contains("AI2");
    }

    private static bool ValidateAISetResponse(string aiResponse)
    {
        return !aiResponse.Contains("?");
    }

    private void CheckHeartBeat(object state)
    {
        _ = Task.Run(async () =>
        {
            if (_isConnecting)
            {
                return;
            }
            var psCommandResponse = await SendAsync(PsCommand, CancellationToken.None).ConfigureAwait(false);
            if (!ValidatePsCommandResponse(psCommandResponse))
            {
                _isConnected = false;
            }
        });
    }

    private bool ValidatePsCommandResponse(string psCommandResponse)
    {
        if (_isAICommandEnabled)
        {
            return !psCommandResponse.Contains("?");
        }
        else
        {
            return psCommandResponse.Contains("PS1");
        }
    }
}
