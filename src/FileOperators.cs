using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace LudusaviPlaynite
{

    public class FileOperators
    {
        /// <summary>
        /// Copy directories method from https://docs.microsoft.com/en-us/dotnet/standard/io/how-to-copy-directories
        /// </summary>
        public static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the source directory.
            var sourceDir = new DirectoryInfo(sourceDirName);

            if (!sourceDir.Exists)
                throw new DirectoryNotFoundException(
                    $"Source directory does not exist or could not be found {sourceDirName}");

            // If the destination directory doesn't exist, create it.
            Directory.CreateDirectory(destDirName);

            // Copy files of the source directory.
            var files = sourceDir.GetFiles();
            foreach (var file in files)
            {
                string path = Path.Combine(destDirName, file.Name);
                file.CopyTo(path, true);
            }

            //copy subdirectories to the destination directory.
            if (copySubDirs)
            {
                var subDirectories = sourceDir.GetDirectories();
                foreach (var dir in subDirectories)
                {
                    string path = Path.Combine(destDirName, dir.Name);
                    DirectoryCopy(dir.FullName, path, copySubDirs);
                }
            }
        }

        /// <summary>
        /// Delete a directory from https://stackoverflow.com/a/44324346
        /// uses Async and retries to take into account File handling errors
        /// </summary>
        /// <param name="directoryPath"></param>
        /// <param name="maxRetries"></param>
        /// <param name="millisecondsDelay"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static async Task<bool> TryDeleteDirectory(
            string directoryPath,
            int maxRetries = 10,
            int millisecondsDelay = 30)
        {
            if (directoryPath == null)
                throw new ArgumentNullException(directoryPath);
            if (maxRetries < 1)
                throw new ArgumentOutOfRangeException(nameof(maxRetries));
            if (millisecondsDelay < 1)
                throw new ArgumentOutOfRangeException(nameof(millisecondsDelay));

            for (int i = 0; i < maxRetries; ++i)
            {
                try
                {
                    if (Directory.Exists(directoryPath))
                    {
                        Directory.Delete(directoryPath, true);
                    }

                    if (!Directory.Exists(directoryPath)) return true;
                }
                catch (IOException)
                {
                    await Task.Delay(millisecondsDelay);
                }
                catch (UnauthorizedAccessException)
                {
                    await Task.Delay(millisecondsDelay);
                }
            }

            return false;
        }
    }
}
