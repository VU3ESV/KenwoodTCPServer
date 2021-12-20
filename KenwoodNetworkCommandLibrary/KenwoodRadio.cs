using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;

namespace Kenwood
{
    public class KenwoodRadio : IKenwoodRadio, IDisposable
    {
        private static Socket _clientSocket;
        private static readonly Encoding _encoding = Encoding.Unicode;
        private readonly string _userName;
        private readonly string _password;
        private readonly IPAddress _radioAddress;
        private readonly int _port;
        private readonly RadioType _radioType;
        private bool _isConnected;
        private readonly IPEndPoint _ipEndpoint;
        private readonly bool _isAdmin = true;
        private const string ConnectCommand = "##CN;";
        private const string Ts990IdCommand = "##ID{0}{1}{2}{3};";
        private const string Ts890IdCommand = "##ID{0}{1}{2}{3}{4};";

        /// <summary>
        /// Crete a new instance of the KenwoodRadio
        /// </summary>
        /// <param name="radioAddress">IP Address of the Radio</param>
        /// <param name="port">Port Number of the Radio, 60000 for the TS990</param>
        /// <param name="userName">UserName configured in ARCP990</param>
        /// <param name="password">Password configured in ARCP990</param>
        /// <param name="radioType">Radio type</param>
        private KenwoodRadio(string radioAddress, int port, string userName, string password, RadioType radioType)
        {
            _radioAddress = IPAddress.Parse(radioAddress);
            _port = port;
            _userName = userName;
            _password = password;
            _radioType = radioType;
            _ipEndpoint = new IPEndPoint(_radioAddress, _port == 0 ? 60000 : _port);
            _clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        /// <summary>
        /// Crete a new instance of the KenwoodRadio
        /// </summary>
        /// <param name="radioAddress">IP Address of the Radio</param>
        /// <param name="port">Port Number of the Radio, 60000 for the TS990</param>
        /// <param name="userName">UserName configured in ARCP990</param>
        /// <param name="password">Password configured in ARCP990</param>
        /// <param name="radioType">Radio type</param>
        /// <returns> An instance of the Kenwood Radio</returns>
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
            while (!_clientSocket.Connected)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    _isConnected = false;
                    return false;
                }

                try
                {
                    await _clientSocket.ConnectAsync(_ipEndpoint, cancellationToken);
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
                return false;
            }

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
                var receivedData = await SendAsync(command, cancellationToken);
                receivePayLoad.Add(receivedData);
            }

            return receivePayLoad;
        }

        public async Task<string> SendAsync(string command, CancellationToken cancellationToken)
        {
            var receivePayLoad = string.Empty;
            if (_clientSocket == null || _clientSocket.Connected == false)
            {
                return receivePayLoad;
            }

            byte[] sendBuffer = _encoding.GetBytes(command);
            byte[] receiveBuffer = new byte[1024];
            var sendLength = await _clientSocket.SendAsync(sendBuffer, SocketFlags.None, cancellationToken);
            if (sendLength == 0)
            {
                // Unable to send the data to radio, may be a network Error or Radio is Off :)
                return receivePayLoad;
            }
            var receiveLength = await _clientSocket.ReceiveAsync(receiveBuffer, SocketFlags.None, cancellationToken);
            byte[] data = new byte[receiveLength];
            Array.Copy(receiveBuffer, data, receiveLength);
            var receivedData = _encoding.GetString(data);
            return receivedData;
        }
        public void Dispose()
        {
            _clientSocket?.Shutdown(SocketShutdown.Both);
            _clientSocket?.Close();
            _clientSocket?.Dispose();
        }

        private static bool ValidateCNResponse(string cnResponse)
        {
            return cnResponse.Contains("##CN1");
        }

        private static bool ValidateIdResponse(string idResponse)
        {
            return idResponse.Contains("##ID1");
        }
    }
}
