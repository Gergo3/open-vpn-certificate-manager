using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Gergo3.OpenVPNCertificateManager;

public class Server
{
    public const int CaLifeTime = 11;
    public const int ServerLifeTime = 10;
    public const int ClientLifeTime = 2;
    
    [Key]
    public Guid Id { get; private set; }
    [MaxLength(100)]
    [Required]
    public string Name { get; private set; }
    [MaxLength(100)]
    [Required]
    public string Domain { get; private set; }
    
    [Required]
    public int Port { get; private set; }
    
    [Required]
    public Protocol Protocol { get; private set; }
    
    [Required]
    public Interface Interface { get; private set; }
    
    [NotMapped]
    public string? Password { private get; set; }
    
    [InverseProperty(nameof(User.Server))]
    public ICollection<User> Users { get; private set; }

    [Required]
    public string CaCertString { get; private set; }

    /// <summary>
    /// Ca certificate in PCKS format
    /// </summary>
    /// <exception cref="PasswordNotSetException">Thrown when <see cref="Password"/> is not set</exception>
    [NotMapped]
    public X509Certificate2 CaCert =>
        field ??= X509CertificateLoader.LoadPkcs12(Convert.FromBase64String(CaCertString),Password ?? throw new PasswordNotSetException());
    
    [Required]
    public string ServerCertString { get; private set; }
    /// <summary>
    /// Server certificate in PCKS format
    /// </summary>
    /// <exception cref="PasswordNotSetException">Thrown when <see cref="Password"/> is not set</exception>
    [NotMapped]
    public X509Certificate2 ServerCert =>
        field ??= X509CertificateLoader.LoadPkcs12(
            Convert.FromBase64String(ServerCertString),
            Password ?? throw new PasswordNotSetException(),
            X509KeyStorageFlags.Exportable |
            X509KeyStorageFlags.EphemeralKeySet,
            Pkcs12LoaderLimits.Defaults);
    
    
    /// <summary>
    /// Ca certificate in Crt format
    /// </summary>
    /// <exception cref="PasswordNotSetException">Thrown when <see cref="Password"/> is not set</exception>
    [NotMapped]
    public string CaCrt => 
        "-----BEGIN CERTIFICATE-----\n" +
        Convert.ToBase64String(
            CaCert.Export(X509ContentType.Cert),
            Base64FormattingOptions.InsertLineBreaks) +
        "\n-----END CERTIFICATE-----";
    
    /// <summary>
    /// Server certificate in Crt format
    /// </summary>
    /// <exception cref="PasswordNotSetException">Thrown when <see cref="Password"/> is not set</exception>
    [NotMapped]
    public string ServerCrt => 
        "-----BEGIN CERTIFICATE-----\n" +
        Convert.ToBase64String(
            ServerCert.Export(X509ContentType.Cert),
            Base64FormattingOptions.InsertLineBreaks) +
        "\n-----END CERTIFICATE-----";
    
    /// <summary>
    /// Server key
    /// </summary>
    /// <exception cref="PasswordNotSetException">Thrown when <see cref="Password"/> is not set</exception>
    [NotMapped]
    public string ServerKey => 
        "-----BEGIN PRIVATE KEY-----\n" +
        Convert.ToBase64String(
        ServerCert
            .GetRSAPrivateKey()?
            .ExportPkcs8PrivateKey() ??  throw new FormatException("Server is missing a private key."),
        Base64FormattingOptions.InsertLineBreaks) +
        "\n-----END PRIVATE KEY-----";
    
    /// <summary>
    /// Ca key
    /// </summary>
    /// <exception cref="PasswordNotSetException">Thrown when <see cref="Password"/> is not set</exception>
    [NotMapped]
    public string CaKey => 
        "-----BEGIN PRIVATE KEY-----\n" +
        Convert.ToBase64String(
            CaCert
                .GetRSAPrivateKey()?
                .ExportPkcs8PrivateKey() ??  throw new FormatException("Server is missing a private key."),
            Base64FormattingOptions.InsertLineBreaks) +
        "\n-----END PRIVATE KEY-----";

    /// <summary>
    /// Creates a new <see cref="CertificateRequest"/>
    /// </summary>
    /// <param name="name">Name of the certificate holder</param>
    /// <param name="isCa">Whether the certificate is a ca</param>
    /// <returns>A new <see cref="CertificateRequest"/></returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="name"/> is null, empty, or consists only of whitespace</exception>
    private static CertificateRequest CreateCertificateRequest(string name, bool isCa = false)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        
        RSA key = RSA.Create(2048);
        
        CertificateRequest request = new(
            $"CN={name}",
            key,
            HashAlgorithmName.SHA256,
            RSASignaturePadding.Pkcs1);
        
        request.CertificateExtensions.Add(
            new X509BasicConstraintsExtension(isCa, false, 0, true));
        request.CertificateExtensions.Add(
            new X509SubjectKeyIdentifierExtension(request.PublicKey, false));

        return request;
    }

    /// <summary>
    /// Creates a new <see cref="X509Certificate2"/> signed by <see cref="CaCert"/>
    /// </summary>
    /// <param name="name">Name of the certificate holder</param>
    /// <param name="isServer">Whether the certificate is a server</param>
    /// <returns>A new <see cref="X509Certificate"/></returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="name"/> is null, empty, or consists only of whitespace; or if <see cref="Domain"/> is null or empty</exception>
    /// <exception cref="PasswordNotSetException"><see cref="Password"/> is not set</exception>
    private X509Certificate2 CreateCertificate(string name, bool isServer = false)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        
        CertificateRequest request =
            CreateCertificateRequest(name, false);

        request.CertificateExtensions.Add(
            new X509KeyUsageExtension(
                X509KeyUsageFlags.DigitalSignature |
                (isServer ? X509KeyUsageFlags.KeyEncipherment : 0),
                false));
        
        // Extended usage: SERVER AUTH
        request.CertificateExtensions.Add(
            new X509EnhancedKeyUsageExtension(
                new OidCollection {
                    new Oid(isServer ? "1.3.6.1.5.5.7.3.1" : "1.3.6.1.5.5.7.3.2")
                },
                false));

        // SAN (important!)
        if (isServer)
        {
            try
            {
                var san = new SubjectAlternativeNameBuilder();
                san.AddDnsName(Domain);
                request.CertificateExtensions.Add(san.Build());
            }
            catch (ArgumentOutOfRangeException e)
            {
                throw new ArgumentException(e.Message);
            }
        }
        
        RSA privateKey =
            request.CreateSelfSigned(
                DateTimeOffset.Now.AddDays(1),
                DateTimeOffset.Now.AddYears(1)).GetRSAPrivateKey() 
            ?? throw new FormatException("request missing private key");

        return request.Create(
            CaCert,
            DateTimeOffset.Now,
            DateTimeOffset.Now.AddYears(isServer ? ServerLifeTime : ClientLifeTime),
            Guid.NewGuid().ToByteArray())
            .CopyWithPrivateKey(privateKey);
    }

    //for EntityFramework
    private Server() {}
    public Server(string name, string domain, string password, Interface nic, Protocol protocol, int port)
    {
        Id = Guid.NewGuid();
        Name = name;
        Domain = domain;
        Password = password;
        Interface = nic;
        Protocol = protocol;
        Port = port;
        
        CaCertString =
            Convert.ToBase64String(
            CreateCertificateRequest($"{name}-ca", true)
            .CreateSelfSigned(
            DateTimeOffset.Now,
            DateTimeOffset.Now.AddYears(CaLifeTime))
            .Export(X509ContentType.Pfx, Password)
            );

        ServerCertString =
            Convert.ToBase64String(
                CreateCertificate($"{name}-server", true)
                    .Export(X509ContentType.Pfx, Password));
    }

    /// <summary>
    /// Creates a new <see cref="User"/> object of the server
    /// </summary>
    /// <param name="name">Name of the user</param>
    /// <returns>A new <see cref="User"/></returns>
    /// <exception cref="ArgumentException">Thrown if <see cref="Domain"/> or <paramref name="name"/> is null or empty</exception>
    /// <exception cref="PasswordNotSetException"><see cref="Password"/> is not set</exception>
    public User CreateUser(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        
        User user = new();
        
        user.Username = name;
        user.ServerId = Id;

        user.CertString =
            Convert.ToBase64String(
                CreateCertificate($"{Name}-{name}", false)
                    .Export(X509ContentType.Pfx, Password));

        
        return user;
    }
}