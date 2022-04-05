namespace Kenwood;

public class RadioPort
{
    public string Comport { get; set; }
    public int BaudRate { get; set; }
    public Parity Parity { get; set; }
    public int DataBits { get; set; }
    public StopBits StopBits { get; set; }
    public string RTS { get; set; }
    public string DTR { get; set; }
    public Handshake Handshake { get; set; }
    public int ReadTimeout { get; set; }
    public int WriteTimeout { get; set; }
}
