using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Gergo3.OpenVPNCertificateManager;

public class Server
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Domain { get; set; }
    
    public string Password { private get; set; }
    
    [InverseProperty(nameof(User.ServerId))]
    public ICollection<User> Users { get; set; }

    public string CaCertString { get; set; }

    [NotMapped]
    public X509Certificate2 CaCert =>
        field ??= X509CertificateLoader.LoadPkcs12(Convert.FromBase64String(CaCertString),Password);
    public string ServerCertString { get; set; }
    [NotMapped]
    public X509Certificate2 ServerCert =>
        field ??= X509CertificateLoader.LoadPkcs12(Convert.FromBase64String(ServerCertString),Password);


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
    public X509Certificate2 CreateCertificate(string name, bool isServer)
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

        return request.Create(
            CaCert,
            DateTimeOffset.Now,
            DateTimeOffset.Now.AddYears(1),
            Guid.NewGuid().ToByteArray());
    }

    public Server(string name, string domain)
    {
        Id = Guid.NewGuid();
        Name = name;
        Domain = domain;
        
        CaCert =
            CreateCertificateRequest($"{name}-ca", true)
            .CreateSelfSigned(
            DateTimeOffset.Now,
            DateTimeOffset.Now.AddYears(1));


        ServerCert = CreateCertificate($"{name}-server", true);
    }
}