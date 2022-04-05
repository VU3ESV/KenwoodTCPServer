namespace Kenwood;

public interface IKenwoodRadio
{
    Task<bool> ConnectAsync(CancellationToken cancellationToken);

    Task<string> SendAsync(string command, CancellationToken cancellationToken);

    Task<List<string>> SendAsync(List<string> commands, CancellationToken cancellationToken);

    Task DisconnectAsync();

    RadioType RadioType { get; }

    bool IsConnected { get; }
}
