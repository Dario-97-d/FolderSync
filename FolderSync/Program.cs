using System.Text;
using System.Timers;
using FolderSync.ArgsValidation;
using FolderSync.FileUtilities;

namespace FolderSync
{
    internal class Program
    {
        private static readonly FolderSync _folderSync = new();
        private static readonly SyncTimer _syncTimer = new();

        static void Main(string[] args)
        {
            if (args.Length != 4)
            {
                Console.WriteLine("Usage: FolderSync <source_path> <replica_path> <log_file_path> <sync_interval>");
                return;
            }

            var validateArgs = Validate.Args(args);
            if (!validateArgs.IsValid)
            {
                Console.WriteLine(validateArgs.InvalidMessage);
                return;
            }

            Console.WriteLine("-- FolderSync --");

            // Configure Logging early on so that any file system operation is logged.
            Logger.Configure(validateArgs.LogFilePath);

            // Set folders' paths before doing folder setup.
            ConfigureFolderSync(validateArgs.SourcePath, validateArgs.ReplicaPath);

            // Ensure creation of Source and Replica folders and Log file.
            if (!SetupFoldersAndLogFile()) return;

            // Configure resource cleanup in case program is unexpectedly terminated.
            ConfigureGracefulTermination();

            ConfigureSyncTimer(validateArgs.SyncInterval, OnTimedEvent);

            OutputMessageReadyToSync();

            // Initial sync.
            _folderSync.SyncFolders();

            /*
             * The program will now sync folders periodically on the given interval.
             */

            // Keep program running until <Esc> is pressed.
            while (true)
            {
                Console.WriteLine("Press Esc to terminate program.");

                if (Console.ReadKey().Key == ConsoleKey.Escape)
                    break;
            }

            // -- Program terminates --

            CleanupResources();
        }

        #region Events

        private static void Console_CancelKeyPress(object? sender, ConsoleCancelEventArgs e)
        {
            CleanupResources();
        }

        private static void OnTimedEvent(object? source, ElapsedEventArgs e)
        {
            _folderSync.SyncFolders();
        }

        #endregion Events


        private static void CleanupResources()
        {
            _syncTimer?.Dispose();
        }

        private static void ConfigureFolderSync(string sourcePath, string targetPath)
        {
            _folderSync.Configure(sourcePath, targetPath);
        }

        private static void ConfigureGracefulTermination()
        {
            Console.CancelKeyPress += new ConsoleCancelEventHandler(Console_CancelKeyPress);
        }

        private static void ConfigureSyncTimer(int interval, ElapsedEventHandler elapsedHandler)
        {
            _syncTimer.Configure(interval, elapsedHandler);
        }

        private static void OutputMessageReadyToSync()
        {
            var outputReadyToSync = new StringBuilder();

            outputReadyToSync.AppendLine("Program is ready for synchronization.");
            outputReadyToSync.AppendLine($"Source path: {_folderSync.SourcePath}");
            outputReadyToSync.AppendLine($"Replica path: {_folderSync.ReplicaPath}");
            outputReadyToSync.AppendLine($"Log file path: {Logger.FilePath}");
            outputReadyToSync.AppendLine($"Synchronization interval: {_syncTimer.IntervalSeconds} seconds.");

            Console.WriteLine($"{outputReadyToSync}");
            Logger.Log($"{outputReadyToSync}");
        }

        /// <summary>
        /// Ensures the creation of the Log file and the Source and Replica folders.
        /// </summary>
        /// <returns><see langword="true"/> if all items were created; otherwise, <see langword="false"/>.</returns>
        /// <remarks>
        /// This method requires the <see cref="Logger"/> and the <see cref="FolderSync"/> object to be configured
        /// in order to ensure the creation of the Log file and the working Folders.
        /// </remarks>
        private static bool SetupFoldersAndLogFile()
        {
            if (!Logger.IsConfigured)
            {
                Console.WriteLine("Could not ensure the creation of the Log file because the Logger is not configured.");
                return false;
            }

            if (!_folderSync.IsConfigured)
            {
                Console.WriteLine("Could not ensure the creation of the working folders because the FolderSync object is not configured.");
                return false;
            }

            /*
             * Create Log file before creating the folders,
             * so that folder creation is logged.
             */

            return
                FileSystemSetup.EnsureLogFileExists(Logger.FilePath)
                &&
                FileSystemSetup.EnsureSourceDirectoryExists(_folderSync.SourcePath)
                &&
                FileSystemSetup.EnsureReplicaDirectoryExists(_folderSync.ReplicaPath);
        }
    }
}
