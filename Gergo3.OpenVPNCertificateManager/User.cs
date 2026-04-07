using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography.X509Certificates;

namespace Gergo3.OpenVPNCertificateManager;

public class User
{
    [Key]
    public Guid Id {get; set;}
    [MaxLength(100)]
    [Required]
    public string Username {get; set;}
    [Required]
    public Guid ServerId {get; set;}
    
    [ForeignKey(nameof(ServerId))]
    public Server Server {get; set;}
    
    
    
    [Required]
    public string CaCertString { get; set; }

    [NotMapped]
    public X509Certificate2 CaCert =>
        field ??= X509CertificateLoader.LoadPkcs12(Convert.FromBase64String(CaCertString),Password);

    public string? Password { get; set; }

    [Required]
    public string ServerCertString { get; set; }
    [NotMapped]
    public X509Certificate2 ServerCert =>
        field ??= X509CertificateLoader.LoadPkcs12(Convert.FromBase64String(ServerCertString),Password);
    
}