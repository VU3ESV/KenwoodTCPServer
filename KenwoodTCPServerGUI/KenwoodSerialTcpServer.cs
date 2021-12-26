using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Kenwood;

namespace KenwoodTCP
{
    public class KenwoodSerialTcpServer : IKenwoodTcpServer, IDisposable
    {
        private static Socket _serverSocket;
        private static List<Socket> _clientSockets;
        private static readonly byte[] _buffer = new byte[1024];
        private static KenwoodSerialRadio _kenwoodRadio;
        private static readonly Encoding _encoding = Encoding.Unicode;
        private readonly RadioPort _radioPort;       
        private readonly RadioType _radioType;
        private readonly int _tcpPort;
        private readonly int _backlog;
        private volatile bool _connected;
        private readonly Timer _autoConnectTimer;

        public event EventHandler<EventArgs> AppReStart;

        /// <summary>
        /// Crete a new instance of the KenwoodRadio
        /// </summary>
        /// <param name="radioPort">Radio Port Information</param>       
        /// <param name="radioType">Radio type</param>

        private KenwoodSerialTcpServer(RadioPort radioPort,
                                       RadioType radioType,
                                       int tcpPort,
                                       int backlog)
        {
            _radioPort = radioPort;

            _radioType = radioType;
            _tcpPort = tcpPort;
            _backlog = backlog;
            _kenwoodRadio = KenwoodSerialRadio.Create(_radioPort,
                                                      _radioType);

            _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _clientSockets = new List<Socket>();
            _autoConnectTimer = new Timer(new TimerCallback(AutoConnct), this, 10000, 10000);
        }

        /// <summary>
        /// Crete a new instance of the KenwoodRadio
        /// </summary>
        /// <param name="radioPort">Com Port Where the Radio is connected</param>   
        /// <param name="radioType">Radio type</param>
        /// <param name="tcpPort">TcpPort</param>
        /// <param name="backlog">backlog</param>
        /// <returns> An instance of the KENWOOD Radio</returns>
        public static KenwoodSerialTcpServer Create(RadioPort radioPort,
                                                    RadioType radioType,
                                                    int tcpPort = 7355,
                                                    int backlog = 100)
        {
            KenwoodSerialTcpServer kenwoodTcpServer = new KenwoodSerialTcpServer(radioPort,
                                                                                 radioType,
                                                                                 tcpPort,
                                                                                 backlog);
            return kenwoodTcpServer;
        }

        public async Task<bool> InitializeAsync()
        {
            try
            {
                _connected = await _kenwoodRadio.ConnectAsync(CancellationToken.None);
                if (!_connected)
                {
                    return false;
                }

                _serverSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                _serverSocket.Bind(new IPEndPoint(IPAddress.Any, _tcpPort));
                _serverSocket.Listen(_backlog);
                _serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void AcceptCallback(IAsyncResult ar)
        {
            Socket socket = _serverSocket.EndAccept(ar);
            _clientSockets.Add(socket);
            socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
            _serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);
        }

        private static async void ReceiveCallback(IAsyncResult ar)
        {
            Socket socket = (Socket)ar.AsyncState;
            try
            {
                int received = socket.EndReceive(ar, out SocketError socketError);
                if (received > 0)
                {
                    if (socketError == SocketError.Success)
                    {
                        byte[] dataBuffer = new byte[received];
                        Array.Copy(_buffer, dataBuffer, received);
                        var data = Encoding.ASCII.GetString(dataBuffer);
                        var response = await _kenwoodRadio?.SendAsync(data, cancellationToken: CancellationToken.None);
                        var responseData = !string.IsNullOrEmpty(response)? Encoding.ASCII.GetBytes(response)
                                                                          : Encoding.ASCII.GetBytes(string.Empty);
                        if (socket.Connected)
                        {
                            socket?.BeginSend(responseData, 0, responseData.Length, SocketFlags.None, new AsyncCallback(SendCallback), socket);

                            socket?.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
                        }
                    }
                    else
                    {
                        _clientSockets.Remove(socket);
                        Close(socket);
                    }
                }
                else
                {
                    _clientSockets.Remove(socket);
                    Close(socket);
                }
            }
            catch (SocketException)
            {
                _clientSockets.Remove(socket);
                Close(socket);
            }
        }

        private static void SendCallback(IAsyncResult ar)
        {
            Socket socket = (Socket)ar.AsyncState;
            try
            {
                socket.EndSend(ar, out SocketError socketError);
                if (socketError != SocketError.Success)
                {
                    _clientSockets.Remove(socket);
                    Close(socket);
                }
            }
            catch (SocketException)
            {
                _clientSockets.Remove(socket);
                Close(socket);
            }
        }

        private static void Close(Socket socket)
        {
            socket.Dispose();
            socket.Close();
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            _autoConnectTimer?.Dispose();
            _kenwoodRadio?.DisconnectAsync().Wait();
            _kenwoodRadio?.Dispose();

            foreach (var clientSocket in _clientSockets)
            {
                clientSocket?.Shutdown(SocketShutdown.Both);
                clientSocket?.Close();
                clientSocket?.Dispose();
            }

            _serverSocket?.Shutdown(SocketShutdown.Both);
            _serverSocket?.Close();
            _serverSocket?.Dispose();

        }

        public Task EnableAutoReConnect(bool enable)
        {
            if (enable == false)
            {
                _autoConnectTimer.Change(Timeout.Infinite, Timeout.Infinite);
            }
            else
            {
                _autoConnectTimer.Change(10000, 10000);
            }

            return Task.CompletedTask;
        }

        private async void AutoConnct(object state)
        {
            if (!_connected)
            {
                await InitializeAsync();
            }
        }
    }
}
