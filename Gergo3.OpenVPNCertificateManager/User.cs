using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography.X509Certificates;

namespace Gergo3.OpenVPNCertificateManager;

public class User
{
    #pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    [Key]
    public Guid Id {get; init;} = Guid.NewGuid();
    [MaxLength(100)]
    [Required]
    public string Username {get; set;}
    [Required]
    public Guid ServerId {get; init;}
    
    [ForeignKey(nameof(ServerId))]
    public Server Server {get; init;}
    
    [Required]
    public byte[] CertData {get; init;}
    #pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    private X509Certificate2? _certificate;
    /// <summary>
    /// User certificate
    /// </summary>
    /// <exception cref="PasswordNotSetException">Thrown when <see cref="Password"/> is not set</exception>
    [NotMapped]
    public X509Certificate2 Certificate =>
        _certificate ??= X509CertificateLoader.LoadPkcs12(
            CertData,
            Password ?? throw new PasswordNotSetException(),
            X509KeyStorageFlags.Exportable |
            X509KeyStorageFlags.EphemeralKeySet,
            Pkcs12LoaderLimits.Defaults);

    [NotMapped]
    public string? Password
    {
        private get;
        set
        {
            field = value;
            _certificate = null;
        }
    }
    
    /// <summary>
    /// User certificate in Crt format
    /// </summary>
    /// <exception cref="PasswordNotSetException">Thrown when <see cref="Password"/> is not set</exception>
    [NotMapped]
    public string Crt => 
        "-----BEGIN CERTIFICATE-----\n" +
        Convert.ToBase64String(
            Certificate.Export(X509ContentType.Cert),
            Base64FormattingOptions.InsertLineBreaks) +
        "\n-----END CERTIFICATE-----";
    
    /// <summary>
    /// User key
    /// </summary>
    /// <exception cref="PasswordNotSetException">Thrown when <see cref="Password"/> is not set</exception>
    [NotMapped]
    public string Key => 
        "-----BEGIN PRIVATE KEY-----\n" +
        Convert.ToBase64String(
            Certificate
                .GetRSAPrivateKey()?
                .ExportPkcs8PrivateKey() ??  throw new FormatException("Server is missing a private key."),
            Base64FormattingOptions.InsertLineBreaks) +
        "\n-----END PRIVATE KEY-----";
}