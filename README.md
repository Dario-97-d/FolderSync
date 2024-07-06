# FolderSync

This is a .NET 8 Console App for one-way two folder synchronization. The app takes as command-line arguments a source folder path, a replica folder path, a log file path and a syncing interval in seconds.

## FolderSync class

The FolderSync class includes the core logic meant to achieve the goal of the application.  

### Configuration

The instantiated object requires configuration for its functioning. This configuration is done using the given paths for the Source and the Replica folders.

### Single one-way nested Syncing

The syncing process is started so that no two syncing processes are done concurrently. Whenever there is already a process, the new one is cancelled.  
The object then proceeds to Delete obsolete content and to Copy new content, from the Source to the Replica.  
These processes are done recursively so that the whole content of the Source is mirrored in the Replica.

### Symbolic links

If the program finds a symbolic link in the Source folder, it then tries to create a new one, with the same link target path, in the Replica folder.

## Step-by-step working

### Args Validation

The app runs a validation on the given command-line arguments to ensure its proper functioning.
- Both source and replica folder paths are validated as file system paths.
- The log file path is validated as a file system path including a valid file name.
- The sync interval is validated as a positive integer.

### Logging

The app runs a basic logging service on a static class.
Its configuration is done as early as possible to ensure the logging of folder and file operations from the setup onwards.
Logging configuration is checked before attempting any loggin operation.
All logging operations are done sequentially to ensure no two threads attempt to access the log file at the same time.

### Setup

The app runs the configuration of the FolderSync object, responsible for the core functioning of the app.  
It then proceeds to ensure the creation of all necessary items in the file system: Log file, Source folder and Replica folder.

#### User confirmation

As for the Source folder, the user is asked whether he wants the app to create it, if it doesn't exist. This is meant to help the user notice possible mistakes in the given source folder path.

After all this is done, a graceful termination is configured to make sure any resources in use are disposed of in case of an unexpected program termination.  
This is useful to safely manage the lifecycle of the SyncTimer, next in the setup timeline.

### Ready to Sync

When everything is set, the app outputs and logs a message to inform the user.  
An initial sync is done.

### App running

The app will now periodically sync the Source and Replica folders.  
All file/directory creation/copying/deletion operations are logged to the log file.

### Exit prompt

With the app running, the user is given the option of easily terminating the program by pressing the Esc key.
If the Esc key is pressed, the app cleans up the resources in use and terminates.
