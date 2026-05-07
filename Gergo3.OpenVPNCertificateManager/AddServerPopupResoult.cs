namespace Gergo3.OpenVPNCertificateManager;

public struct AddServerPopupResult
{
    public string Name { get; set; }
    public string Domain { get; set; }
    public string Password { get; set; }
    public Interface Interface { get; set; }
    public Protocol Protocol { get; set; }
    public int Port { get; set; }
}