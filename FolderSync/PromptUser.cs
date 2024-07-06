namespace FolderSync
{
    internal static class PromptUser
    {
        /// <summary>
        /// Prompts user on whether to create the source directory.
        /// </summary>
        /// <returns><see langword="true"/>, if the user confirms; otherwise, <see langword="false"/>.</returns>
        public static bool ToCreateSourceFolder()
        {
            while (true)
            {
                Console.WriteLine("Create source directory? (Y/n)");

                string input = Console.ReadLine()?
                    .Trim().ToLowerInvariant() ?? "y";

                if (input.Equals("y"))
                {
                    return true;
                }

                if (input.Equals("n"))
                {
                    return false;
                }

                Console.WriteLine("Invalid input.");
            }
        }
    }
}
