namespace Gergo3.OpenVPNCertificateManager;

public struct AddServerPopupResult
{
    public string Name { get; init; }
    public string Domain { get; init; }
    public string Password { get; init; }
    public Interface Interface { get; init; }
    public Protocol Protocol { get; init; }
    public int Port { get; init; }
}