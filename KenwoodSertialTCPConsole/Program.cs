using System;
using System.Threading.Tasks;
using System.IO.Ports;
using Kenwood;
using KenwoodTCP;

namespace KenwoodSertialTCPConsole
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            Console.Title = "KenwoodSerialTcpServer by VU3ESV (For Windows System)";            
            RadioPort radioComPort = new()
            {
                Comport = "COM19",
                BaudRate = 115200,
                DataBits = 8,
                DTR = "High",
                Parity = Parity.None,
                RTS = "High",
                StopBits = StopBits.One,
                Handshake = Handshake.RequestToSend,
                ReadTimeout = 500,
                WriteTimeout = 500
            };

            IKenwoodTcpServer kenwoodTcpServer = KenwoodSerialTcpServer.Create(radioComPort, RadioType.TS990);

            var initialized = await kenwoodTcpServer.InitializeAsync();
            if (initialized)
            {
                Console.WriteLine("Kenwood Serial TCPServer Started Listening on Port 7355");
            }
            Console.WriteLine("Press Enter to Stop the Kenwood TCPServer");
            Console.ReadLine();
        }
    }
}
