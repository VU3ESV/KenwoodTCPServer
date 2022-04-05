namespace KenwoodTCP;

public interface IKenwoodTcpServer
{
    void Dispose();
    Task<bool> InitializeAsync();

    Task EnableAutoReConnect(bool enable);
}
