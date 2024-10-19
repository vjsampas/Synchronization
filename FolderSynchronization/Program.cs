namespace Synchronization;
using System.IO;
using System;
using System.Threading;

public class FolderSyncronization
{

    static string sourceFolderPath;
    static string targetFolderPath;
    static string logFilePath;
    static int syncInterval;
    public static void Main(string[] args)
    {

        if (args.Length != 4)
        {
            Console.WriteLine("Please provide 4 parameters in following sequence ie. sourceFolderPath, targetFolderPath, logFilePath & syncInterval");
            return;
        }
         sourceFolderPath = args[0];
         targetFolderPath = args[1];
         logFilePath = args[2];
         syncInterval = int.Parse(args[3]) * 1000; //To milliseconds


        while (true)
        {
            try
            {
                Console.WriteLine("Synchronization started...");
                SynchronizeTwoFolders(sourceFolderPath, targetFolderPath);
                Console.WriteLine("Synchronization completed...");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occurred while synchronizing: " + ex.Message);
            }

            Thread.Sleep(syncInterval);

        }

    }

    private static void SynchronizeTwoFolders(string sourceFolderPath, string targetFolderPath)
    {
        //Check if source directory exists and has files or folders
        if (!Directory.Exists(sourceFolderPath) || Directory.GetFiles(sourceFolderPath).Length == 0 || Directory.GetDirectories(sourceFolderPath).Length == 0)
        {
            Console.WriteLine("The source directory either doesn't exist or is empty. Aborting synchronization...");
            throw new DirectoryNotFoundException($"The source directory {sourceFolderPath} either doesn't exist or is empty.");
        }

        //Create target directory, if not exists
        if (!Directory.Exists(targetFolderPath))
        {
            Directory.CreateDirectory(targetFolderPath);
            Log($"Destination root directory created '{targetFolderPath}'");
        }

        //Create all subdirectories inside target path, if not exists
        foreach (string sourceDirectory in Directory.GetDirectories(sourceFolderPath, "*.*", SearchOption.AllDirectories))
        {
            string targetDirPath = sourceDirectory.Replace(sourceFolderPath, targetFolderPath);

            if (!Directory.Exists(targetDirPath))
            {
                Directory.CreateDirectory(targetDirPath);
                Log($"Directory created '{targetDirPath}'");
            }

        }

        //Create all files inside directory, if not exists or are different
        foreach (string sourceFile in Directory.GetFiles(sourceFolderPath, "*.*", SearchOption.AllDirectories))
        {
            string targetFile = sourceFile.Replace(sourceFolderPath, targetFolderPath);

            if (!File.Exists(targetFile))
            {
                File.Copy(sourceFile, targetFile, true);
                Log($"File copied from '{sourceFile}' to '{targetFile}'");
            }
        }

        //Delete all files inside target directory which doesn't exists in source directory
        foreach (string targetFile in Directory.GetFiles(targetFolderPath, "*.*", SearchOption.AllDirectories))
        {
            string sourceFile = targetFile.Replace(targetFolderPath, sourceFolderPath);

            if (!File.Exists(sourceFile))
            {
                File.Delete(targetFile);
                Log($"Deleted file '{targetFile}' from target folder '{targetFolderPath}'");
            }
        }

        //Delete all subdirectories inside target directory which doesn't exists in source directory
        foreach (string targetDirectory in Directory.GetDirectories(targetFolderPath, "*.*", SearchOption.AllDirectories))
        {
            string sourceDirectory = targetDirectory.Replace(targetFolderPath, sourceFolderPath);

            if (!Directory.Exists(sourceDirectory))
            {
                Directory.Delete(targetDirectory);
                Log($"Deleted subdirectory '{targetDirectory}' inside target folder '{targetFolderPath}'");
            }

        }

    }

    private static void Log(string logMessage)
    {
        Console.WriteLine(logMessage);
        //File.AppendAllText(logFilePath, DateTime.Now + logMessage + Environment.NewLine);
    }

}
