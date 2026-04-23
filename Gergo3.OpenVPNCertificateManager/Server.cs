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
    
    [NotMapped]
    public string? Password { private get; set; }
    
    [InverseProperty(nameof(User.Server))]
    public ICollection<User> Users { get; private set; }

    [Required]
    public string CaCertString { get; private set; }

    [NotMapped]
    public X509Certificate2 CaCert =>
        field ??= X509CertificateLoader.LoadPkcs12(Convert.FromBase64String(CaCertString),Password ?? throw new PasswordNotSetException());
    
    [Required]
    public string ServerCertString { get; private set; }
    [NotMapped]
    public X509Certificate2 ServerCert =>
        field ??= X509CertificateLoader.LoadPkcs12(Convert.FromBase64String(ServerCertString),Password ?? throw new PasswordNotSetException());
    
    
    public string CaCrt => 
        "-----BEGIN CERTIFICATE-----\n" +
        Convert.ToBase64String(
            CaCert.Export(X509ContentType.Cert),
            Base64FormattingOptions.InsertLineBreaks) +
        "\n-----END CERTIFICATE-----";
    
    public string ServerCrt => 
        "-----BEGIN CERTIFICATE-----\n" +
        Convert.ToBase64String(
            ServerCert.Export(X509ContentType.Cert),
            Base64FormattingOptions.InsertLineBreaks) +
        "\n-----END CERTIFICATE-----";
    
    public string ServerKey => 
        "-----BEGIN PRIVATE KEY-----\n" +
        Convert.ToBase64String(
        ServerCert
            .GetRSAPrivateKey()?
            .ExportPkcs8PrivateKey() ??  throw new FormatException("Server is missing a private key."),
        Base64FormattingOptions.InsertLineBreaks) +
        "\n-----END PRIVATE KEY-----";
    
    public string CaKey => 
        "-----BEGIN PRIVATE KEY-----\n" +
        Convert.ToBase64String(
            CaCert
                .GetRSAPrivateKey()?
                .ExportPkcs8PrivateKey() ??  throw new FormatException("Server is missing a private key."),
            Base64FormattingOptions.InsertLineBreaks) +
        "\n-----END PRIVATE KEY-----";

    private static CertificateRequest CreateCertificateRequest(string name, bool isCa)
    {
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

    private X509Certificate2 CreateCertificate(string name, bool isServer)
    {
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
            var san = new SubjectAlternativeNameBuilder();
            san.AddDnsName(Domain);
            request.CertificateExtensions.Add(san.Build());
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
    public Server(string name, string domain, string password)
    {
        Id = Guid.NewGuid();
        Name = name;
        Domain = domain;
        Password = password;
        
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

    public User CreateUser(string name)
    {
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