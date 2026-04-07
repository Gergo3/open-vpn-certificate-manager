using System;
using System.IO;

namespace Gergo3.OpenVPNCertificateManager;

public static class AppDir
{
    private const string AppDirName = "Gergo3/OpenVPNCertificateManager";
    
    private static readonly string ApplicationData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), AppDirName);
    
    public static string DbPath => Path.Combine(ApplicationData, "OpenVPNCertificateManager.db");

    public static readonly DirectoryInfo OutputDir = new(Path.Combine(ApplicationData, "output"));

    static AppDir()
    {
        Directory.CreateDirectory(OutputDir.FullName);
    }
}