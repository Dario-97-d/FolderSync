using FolderSync.FileUtilities;

namespace FolderSync
{
    internal static class Logger
    {
        private static readonly object _lock = new();

        public static bool IsConfigured { get; private set; } = false;

        public static string FilePath { get; private set; } = string.Empty;

        public static void Configure(string filePath)
        {
            FilePath = filePath;

            IsConfigured = true;
        }

        public static void Log(string message)
        {
            if (!IsConfigured)
            {
                Console.WriteLine("Could not log: logging is not configured.");
                return;
            }

            string logMessage = $"[{DateTime.UtcNow:u}] {message}";

            try
            {
                lock (_lock)
                {
                    FileSystemSetup.EnsureLogFileExists(FilePath);

                    File.AppendAllText(FilePath, logMessage + Environment.NewLine);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not log to file. Exception message: {ex.Message}. Log message: {logMessage}.");
            }
        }
    }
}
