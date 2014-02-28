using System;
using System.IO;
using System.Reactive;
using System.Reactive.Concurrency;

namespace Alteridem.GitHub.Akavache
{
    /// <summary>
    /// An abstraction for the simple file operations that an IBlobCache can
    /// perform. Create a new instance of this when adapting IBlobCache to
    /// different platforms or backing stores, or for testing purposes.
    /// </summary>
    public interface IFilesystemProvider
    {
        /// <summary>
        /// Open a file on a background thread, with the File object in 'async
        /// mode'. It is critical that this operation is deferred and returns
        /// immediately (i.e. wrapped in an Observable.Start).
        /// </summary>
        /// <param name="path">The path to the file</param>
        /// <param name="mode">The file mode</param>
        /// <param name="access">The required access privileges</param>
        /// <param name="share">The allowed file sharing modes.</param>
        /// <param name="scheduler">The scheduler to schedule the open under.</param>
        /// <returns>A Future result representing the Open file.</returns>
        IObservable<Stream> SafeOpenFileAsync(string path, FileMode mode, FileAccess access, FileShare share, IScheduler scheduler);

        /// <summary>
        /// Create a directory and its parents. If the directory already
        /// exists, this method does nothing (i.e. it does not throw if a
        /// directory exists)
        /// </summary>
        /// <param name="path">The path to create.</param>
        IObservable<Unit> CreateRecursive(string path);

        /// <summary>
        /// Deletes a file.
        /// </summary>
        /// <param name="path">The path to the file</param>
        IObservable<Unit> Delete(string path);

        /// <summary>
        /// Gets the default local machine cache directory (i.e. the one for temporary data)
        /// </summary>
        /// <returns>The default local machine cache directory.</returns>
        string GetDefaultLocalMachineCacheDirectory();

        /// <summary>
        /// Gets the default roaming cache directory (i.e. the one for user settings)
        /// </summary>
        /// <returns>The default roaming cache directory.</returns>
        string GetDefaultRoamingCacheDirectory();
                
        /// <summary>
        /// Gets the default roaming cache directory (i.e. the one for user settings)
        /// </summary>
        /// <returns>The default roaming cache directory.</returns>
        string GetDefaultSecretCacheDirectory();
    }
}