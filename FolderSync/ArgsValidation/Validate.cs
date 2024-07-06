namespace FolderSync.ArgsValidation
{
    /// <summary>
    /// Provides methods for validating command-line arguments for the application.
    /// </summary>
    internal static class Validate
    {
        /// <summary>
        /// Validates arguments based on the application's needs.
        /// </summary>
        /// <returns>
        /// <see cref="ArgsValidation.Args"/> object:
        /// <para>- If <see cref="Args.IsValid"/> is <see langword="true"/>, the validated values.</para>
        /// <para>- If <see cref="Args.IsValid"/> is <see langword="false"/>, an invalid message.</para>
        /// </returns>
        internal static Args Args(string[] args)
        {
            try
            {
                string sourcePath = ValidatePath(args[0], "source_path");
                string replicaPath = ValidatePath(args[1], "replica_path");
                string logFilePath = ValidatePathAndFileName(args[2], "log_file_path");
                int syncInterval = ValidateSyncIntervalSeconds(args[3]);

                return new Args
                {
                    IsValid = true,
                    SourcePath = sourcePath,
                    ReplicaPath = replicaPath,
                    SyncInterval = syncInterval,
                    LogFilePath = logFilePath,
                };
            }
            catch (Exception ex)
            {
                return new Args
                {
                    IsValid = false,
                    InvalidMessage = ex.Message,
                };
            }
        }

        /// <summary>
        /// Validates <paramref name="input"/> as a file system path.
        /// Uses <paramref name="paramName"/> for exception messages.
        /// </summary>
        /// <returns>The validated path.</returns>
        /// <exception cref="ArgumentException"></exception>
        static string ValidatePath(string input, string paramName)
        {
            // Check whether input has invalid path characters.
            if (input.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
            {
                throw new ArgumentException($"Path ({paramName}) has invalid characters.");
            }

            // Check whether the path is absolute.
            if (!Path.IsPathRooted(input))
            {
                throw new ArgumentException($"Path ({paramName}) isn't absolute.");
            }

            return input.ToLowerInvariant();
        }

        /// <summary>
        /// Validates <paramref name="input"/> as a file path.
        /// Uses <paramref name="paramName"/> for exception messages.
        /// </summary>
        /// <returns>The validated file path.</returns>
        /// <exception cref="ArgumentException"></exception>
        static string ValidatePathAndFileName(string input, string paramName)
        {
            input = ValidatePath(input, paramName);

            // Check whether input has file name with invalid characters.
            string fileName = Path.GetFileName(input);

            if (fileName != null && fileName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
            {
                throw new ArgumentException($"File name ({fileName}) has invalid characters.");
            }

            return input.ToLowerInvariant();
        }

        /// <summary>
        /// Validates <paramref name="input"/> as a time interval in seconds.
        /// Uses <paramref name="paramName"/> for exception message.
        /// </summary>
        /// <returns>The <paramref name="input"/> for the time interval in seconds as an integer.</returns>
        /// <exception cref="ArgumentException"></exception>
        static int ValidateSyncIntervalSeconds(string input)
        {
            if (int.TryParse(input, out int result))
            {
                if (result > 0)
                {
                    return result;
                }
            }

            throw new ArgumentException($"The syncing interval must be given in seconds and be a positive integer.");
        }
    }
}
