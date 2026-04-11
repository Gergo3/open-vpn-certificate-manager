using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography.X509Certificates;

namespace Gergo3.OpenVPNCertificateManager;

public class User
{
    [Key]
    public Guid Id {get; set;} = Guid.NewGuid();
    [MaxLength(100)]
    [Required]
    public string Username {get; set;}
    [Required]
    public Guid ServerId {get; set;}
    
    [ForeignKey(nameof(ServerId))]
    public Server Server {get; set;}
    
    [Required]
    public string CertString {get; set;}
    
    [NotMapped]
    public X509Certificate2 Certificate =>
        field ??= X509CertificateLoader.LoadPkcs12(Convert.FromBase64String(CertString),Password ?? throw new PasswordNotSetException());
    
    [NotMapped]
    public string? Password { private get; set;}
}