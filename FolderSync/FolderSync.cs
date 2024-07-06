using FolderSync.FileUtilities;

namespace FolderSync
{
    /// <summary>
    /// Handles one-way synchronization between a source and replica folder,
    /// ensuring files and directories are kept up-to-date, including nested folders.
    /// </summary>
    internal class FolderSync
    {
        private bool _syncing = false;

        public bool IsConfigured { get; private set; } = false;

        public string SourcePath { get; private set; } = null!;

        public string ReplicaPath { get; private set; } = null!;


        public void Configure(string sourcePath, string replicaPath)
        {
            SourcePath = sourcePath;
            ReplicaPath = replicaPath;

            IsConfigured = true;
        }

        /// <summary>
        /// Attempts to one-way synchronize the source and replica folders.
        /// </summary>
        public void SyncFolders()
        {
            if (!IsConfigured)
            {
                string message = $"{nameof(FolderSync)} is not configured.";
                Console.WriteLine(message);
                Logger.Log(message);

                return;
            }

            if (_syncing)
            {
                Console.WriteLine("Current syncing has been cancelled because the previous one is still in progress.");
                return;
            }

            Logger.Log("Synchronization started.");

            try
            {
                _syncing = true;

                // Ensure source and replica folders exist.
                FileSystemSetup.EnsureSourceDirectoryExists(SourcePath);
                FileSystemSetup.EnsureReplicaDirectoryExists(ReplicaPath);

                // Sync contents.
                DeleteObsoleteContent(SourcePath, ReplicaPath);
                CopyNewContent(SourcePath, ReplicaPath);
            }
            catch (Exception ex)
            {
                string errorMessage = $"Error during synchronization: {ex.Message}";
                Console.WriteLine(errorMessage);
                Logger.Log(errorMessage);
            }
            finally
            {
                _syncing = false;
            }

            Logger.Log("Synchronization completed.");
        }

        /// <summary>
        /// Copies new or modified files from the source directory to the target directory,
        /// including nested folders.
        /// <para>
        /// In the case of symbolic links,
        /// this method creates a new symbolic link with the same link target path.
        /// </para>
        /// </summary>
        /// <remarks>
        /// This method may throw exceptions related to file access and I/O operations.
        /// Ensure that it is called within a try-catch block to handle potential exceptions.
        /// </remarks>
        private static void CopyNewContent(string sourceDir, string targetDir)
        {
            // Copy new content from current location.
            foreach (string sourceFilePath in Directory.GetFiles(sourceDir))
            {
                string targetFilePath = Path.Combine(targetDir, Path.GetFileName(sourceFilePath));

                // Continue if file exists at the target directory and is equal to the one at the source.
                if (File.Exists(targetFilePath) && HashHelper.FilesAreEqual(sourceFilePath, targetFilePath))
                {
                    continue;
                }

                // Check if file is a symbolic link.
                var sourceFileInfo = new FileInfo(sourceFilePath);
                if (sourceFileInfo.Attributes.HasFlag(FileAttributes.ReparsePoint))
                {
                    // Try to create symbolic link.
                    var linkTargetPath = sourceFileInfo.LinkTarget;
                    if (linkTargetPath is null)
                    {
                        Logger.Log($"Could not create symbolic link because {sourceFilePath} doesn't have a link target path.");
                    }
                    else if (!File.Exists(linkTargetPath) && !Directory.Exists(linkTargetPath))
                    {
                        Logger.Log($"Could not create symbolic link because the link target path {linkTargetPath} does not exist.");
                    }
                    else
                    {
                        // Delete existing file before creating symbolic link.
                        if (File.Exists(targetFilePath))
                        {
                            File.Delete(targetFilePath);
                        }

                        File.CreateSymbolicLink(targetFilePath, linkTargetPath);
                        Logger.Log($"Created symbolic link at {targetFilePath} to {linkTargetPath}");
                    }

                }
                else
                {
                    // If source file is not a symbolic link, copy it.
                    File.Copy(sourceFilePath, targetFilePath, true);
                    Logger.Log($"Copied file: {sourceFilePath} to {targetFilePath}");
                }
            }

            // Copy new content recursively in nested folders.
            foreach (string sourceSubdirectoryPath in Directory.GetDirectories(sourceDir))
            {
                string targetSubdirectoryPath = Path.Combine(targetDir, Path.GetFileName(sourceSubdirectoryPath));

                if (!Directory.Exists(targetSubdirectoryPath))
                {
                    Directory.CreateDirectory(targetSubdirectoryPath);
                    Logger.Log($"Created directory: {sourceSubdirectoryPath}");
                }

                CopyNewContent(sourceSubdirectoryPath, targetSubdirectoryPath);
            }
        }

        /// <summary>
        /// Deletes files and directories in the target directory that do not exist in the source directory,
        /// including nested folders.
        /// </summary>
        /// <remarks>
        /// This method may throw exceptions related to file access and I/O operations.
        /// Ensure that it is called within a try-catch block to handle potential exceptions.
        /// </remarks>
        private static void DeleteObsoleteContent(string sourceDir, string targetDir)
        {
            // Delete obsolete files from current location.
            foreach (string targetFilePath in Directory.GetFiles(targetDir))
            {
                string sourceFilePath = Path.Combine(sourceDir, Path.GetFileName(targetFilePath));

                if (!File.Exists(sourceFilePath))
                {
                    File.Delete(targetFilePath);
                    Logger.Log($"Deleted file: {targetFilePath}");
                }
            }

            // Recursively delete content from nested folders.
            foreach (string targetSubdirectoryPath in Directory.GetDirectories(targetDir))
            {
                string sourceSubdirectoryPath = Path.Combine(sourceDir, Path.GetFileName(targetSubdirectoryPath));

                if (Directory.Exists(sourceSubdirectoryPath))
                {
                    DeleteObsoleteContent(sourceSubdirectoryPath, targetSubdirectoryPath);
                }
                else
                {
                    Directory.Delete(targetSubdirectoryPath, true);
                    Logger.Log($"Deleted directory: {targetSubdirectoryPath}");
                }
            }
        }
    }
}
