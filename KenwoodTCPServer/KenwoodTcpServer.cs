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
    public class KenwoodTcpServer : IKenwoodTcpServer, IDisposable
    {
        private static Socket _serverSocket;
        private static List<Socket> _clientSockets;
        private static readonly byte[] _buffer = new byte[1024];
        private static KenwoodRadio _kenwoodRadio;
        private static readonly Encoding _encoding = Encoding.Unicode;
        private readonly string _userName;
        private readonly string _password;
        private readonly string _radioAddress;
        private readonly int _port;
        private readonly RadioType _radioType;
        private readonly int _tcpPort;
        private readonly int _backlog;
        private volatile bool _connected;
        private readonly Timer _autoConnectTimer;

        /// <summary>
        /// Crete a new instance of the KenwoodRadio
        /// </summary>
        /// <param name="radioAddress">IP Address of the Radio</param>
        /// <param name="port">Port Number of the Radio, 60000 for the TS990</param>
        /// <param name="userName">UserName configured in ARCP990</param>
        /// <param name="password">Password configured in ARCP990</param>
        /// <param name="radioType">Radio type</param>
        /// /// <param name="tcpPort">TcpPort</param>
        /// <param name="backlog">backlog</param>
        private KenwoodTcpServer(string radioAddress,
                                 int port,
                                 string userName,
                                 string password,
                                 RadioType radioType,
                                 int tcpPort,
                                 int backlog)
        {
            _radioAddress = radioAddress;
            _port = port;
            _userName = userName;
            _password = password;
            _radioType = radioType;
            _tcpPort = tcpPort;
            _backlog = backlog;
            _kenwoodRadio = KenwoodRadio.Create(_radioAddress,
                                                _port,
                                                _userName,
                                                _password,
                                                _radioType);

            _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _clientSockets = new List<Socket>();
            _autoConnectTimer = new Timer(new TimerCallback(AutoConnct), this, 10000, 10000);
        }

        /// <summary>
        /// Crete a new instance of the KenwoodRadio
        /// </summary>
        /// <param name="radioAddress">IP Address of the Radio</param>
        /// <param name="port">Port Number of the Radio, 60000 for the TS990</param>
        /// <param name="userName">UserName configured in ARCP990</param>
        /// <param name="password">Password configured in ARCP990</param>
        /// <param name="radioType">Radio type</param>
        /// <param name="tcpPort">TcpPort</param>
        /// <param name="backlog">backlog</param>
        /// <returns> An instance of the Kenwood Radio</returns>
        public static KenwoodTcpServer Create(string radioAddress,
                                              int port,
                                              string userName,
                                              string password,
                                              RadioType radioType,
                                              int tcpPort = 7355,
                                              int backlog = 100)
        {
            KenwoodTcpServer kenwoodTcpServer = new(radioAddress,
                                                    port,
                                                    userName,
                                                    password,
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
            int received = socket.EndReceive(ar);
            byte[] dataBuffer = new byte[received];
            Array.Copy(_buffer, dataBuffer, received);
            var data = Encoding.ASCII.GetString(dataBuffer);
            var response = await _kenwoodRadio.SendAsync(data, cancellationToken: CancellationToken.None);
            var responseData = Encoding.ASCII.GetBytes(response);
            socket.BeginSend(responseData, 0, responseData.Length, SocketFlags.None, new AsyncCallback(SendCallback), socket);
            socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
        }

        private static void SendCallback(IAsyncResult ar)
        {
            Socket socket = (Socket)ar.AsyncState;
            socket.EndSend(ar);
        }

        public void Dispose()
        {
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
