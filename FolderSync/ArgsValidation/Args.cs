namespace FolderSync.ArgsValidation
{
    internal class Args
    {
        public bool IsValid { get; set; } = false;

        public string InvalidMessage { get; set; } = string.Empty;
        
        public string SourcePath { get; set; } = string.Empty;

        public string ReplicaPath { get; set; } = string.Empty;

        public string LogFilePath { get; set; } = string.Empty;

        public int SyncInterval { get; set; } = -1;
    }
}
