namespace FolderSync.FileUtilities
{
    /// <summary>
    /// Provides utility methods to ensure the existence of log files and directories required for folder synchronization.
    /// </summary>
    internal static class FileSystemSetup
    {
        public static bool EnsureLogFileExists(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    Console.WriteLine($"Log file ({filePath}) doesn't exist. Creating it...");

                    // Create and close the file.
                    File.Create(filePath).Dispose();

                    Console.WriteLine("Log file created.");
                    Logger.Log("Log file created.");
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not create log file. Exception message: {ex.Message}");
                return false;
            }
        }

        public static bool EnsureReplicaDirectoryExists(string path)
        {
            try
            {
                if (!Directory.Exists(path))
                {
                    Console.WriteLine($"The replica directory ({path}) doesn't exist. Creating it...");

                    Directory.CreateDirectory(path);

                    Console.WriteLine("The replica directory has been created.");
                    Logger.Log($"The replica directory ({path}) has been created.");
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Could not create directory.");
                Logger.Log($"Could not create replica directory ({path}). Exception message: {ex.Message}");

                return false;
            }
        }

        public static bool EnsureSourceDirectoryExists(string path)
        {
            /*
             * Prompt user if directory doesn't exist.
             */

            try
            {
                if (!Directory.Exists(path))
                {
                    Console.WriteLine($"The given source directory ({path}) doesn't exist.");

                    // Get user confirmation on creating source folder.
                    var createSourceFolder = PromptUser.ToCreateSourceFolder();
                    if (!createSourceFolder)
                    {
                        Console.WriteLine("Source directory is required.");
                        return false;
                    }

                    Directory.CreateDirectory(path);

                    Console.WriteLine("The source directory has been created.");
                    Logger.Log($"The source directory ({path}) has been created.");
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Could not create directory.");
                Logger.Log($"Could not create source directory {path}. Exception message: {ex.Message}");

                return false;
            }
        }
    }
}
