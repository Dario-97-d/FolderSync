using System.Security.Cryptography;

namespace FolderSync.FileUtilities
{
    internal static class HashHelper
    {
        /// <summary>
        /// Checks whether two files are equal using <see cref="MD5"/> hashing.
        /// </summary>
        /// <returns><see langword="true"/> if the <see cref="MD5"/> hashes for the files are equal;
        /// otherwise, <see langword="false"/>.</returns>
        /// <remarks>
        /// This method may throw exceptions related to file access and I/O operations.
        /// Ensure that it is called within a try-catch block to handle potential exceptions.
        /// </remarks>
        public static bool FilesAreEqual(string filePath1, string filePath2)
        {
            using var md5 = MD5.Create();
            using var stream1 = File.OpenRead(filePath1);
            using var stream2 = File.OpenRead(filePath2);

            byte[] hash1 = md5.ComputeHash(stream1);
            byte[] hash2 = md5.ComputeHash(stream2);

            return hash1.SequenceEqual(hash2);
        }
    }
}
