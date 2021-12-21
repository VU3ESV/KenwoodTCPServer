using System;
using System.Threading.Tasks;

namespace KenwoodTCP
{
    public interface IKenwoodTcpServer
    {
        void Dispose();
        Task<bool> InitializeAsync();

        Task EnableAutoReConnect(bool enable);

        event EventHandler<EventArgs> AppReStart;
    }
}