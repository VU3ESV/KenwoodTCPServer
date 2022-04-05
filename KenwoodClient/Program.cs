using Kenwood;
using KenwoodTCP;
using System;
using System.Threading.Tasks;

namespace KenwoodClient;

class Program
{
    public static async Task Main(string[] args)
    {
        Console.Title = "KenwoodTcpServer by VU3ESV";
        var radioIpAddress = "192.168.1.102";
        var radioPort = 60000;
        var userName = "admin";
        var password = "Kenwood";
        IKenwoodTcpServer kenwoodTcpServer = KenwoodTcpServer.Create(radioIpAddress,
                                                                     radioPort,
                                                                     userName,
                                                                     password,
                                                                     RadioType.TS990,
                                                                     7355,
                                                                     100);

        var initialized = await kenwoodTcpServer.InitializeAsync().ConfigureAwait(false);
        if (initialized)
        {
            Console.WriteLine("Kenwood TCPServer Started Listening on Port 7355");
        }
        Console.WriteLine("Press Enter to Stop the Kenwood TCPServer");
        Console.ReadLine();
    }
}
