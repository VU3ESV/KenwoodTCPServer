using System;
using System.Threading.Tasks;
using Kenwood;
using KenwoodTCP;

namespace KenwoodClient
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            Console.Title = "KenwoodTcpServer by VU3ESV";
            var radioIpAddress = "192.168.1.102";
            var radioPort = 60000;
            var userName = "admin";
            var password = "Kenwood";
            //IKenwoodTcpServer kenwoodTcpServer = KenwoodTcpServer.Create(radioIpAddress,
            //                                                             radioPort,
            //                                                             userName,
            //                                                             password,
            //                                                             RadioType.TS990,
            //                                                             7355,
            //                                                             100);

            RadioPort radioComPort = new RadioPort
            {
                Comport = "COM19",
                BaudRate = 115200,
                DataBits = 8,
                DTR = "High",
                Parity = System.IO.Ports.Parity.None,
                RTS = "High",
                StopBits = System.IO.Ports.StopBits.One,
                Handshake = System.IO.Ports.Handshake.RequestToSend,
                ReadTimeout = 500,
                WriteTimeout = 500
            };

            IKenwoodTcpServer kenwoodTcpServer = KenwoodSerialTcpServer.Create(radioComPort, RadioType.TS990);

            var initialized = await kenwoodTcpServer.InitializeAsync();
            if(initialized)
            {
                Console.WriteLine("Kenwood TCPServer Started Listening on Port 7355");
            }
            Console.WriteLine("Press Enter to Stop the Kenwood TCPServer");
            Console.ReadLine();            
        }        
    }
}
