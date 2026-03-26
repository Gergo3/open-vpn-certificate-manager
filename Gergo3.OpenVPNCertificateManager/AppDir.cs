using System;
using System.IO;

namespace Gergo3.OpenVPNCertificateManager;

public class AppDir
{
    private const string AppDirName = "Gergo3/OpenVPNCertificateManager";
    
    private static readonly string ApplicationData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), AppDirName);

    public static readonly DirectoryInfo OutputDir = new(Path.Combine(ApplicationData, "output"));
}