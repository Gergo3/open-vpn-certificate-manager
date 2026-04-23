using System;
using System.IO;

namespace Gergo3.OpenVPNCertificateManager;

public static class AppDir
{
    private const string AppDirName = "Gergo3/OpenVPNCertificateManager";
    
    private static readonly string ApplicationData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), AppDirName);
    
    public static string DbPath => Path.Combine(ApplicationData, "OpenVPNCertificateManager.db");

    public static readonly DirectoryInfo OutputDir = new(Path.Combine(ApplicationData, "output"));
    
    private static string TempDirPath => Path.Combine(Path.GetTempPath(), AppDirName);
    
    public static DirectoryInfo TempDir
    {
        get
        {
            var dir = new DirectoryInfo(TempDirPath);
            if (!dir.Exists) dir.Create();
            return field ??= dir;
        }
    }

    static AppDir()
    {
        Directory.CreateDirectory(OutputDir.FullName);
        
        if (Directory.Exists(TempDirPath)) TempDir?.Delete(true);
        
        AppDomain.CurrentDomain.ProcessExit += (s, e) =>
        {
            try
            {
                TempDir?.Delete(true);
            }
            catch (Exception exception)
            {
                Console.Error.WriteLine(exception);
            }
        };
    }
}