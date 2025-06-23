using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace FMH.Utils.Files
{
    /// <summary>
    /// Utility class for file operations
    /// </summary>
    public static class FileUtils
    {
        /// <summary>
        /// Downloads a file from the specified URL to the given destination path, with optional progress reporting
        /// </summary>
        /// <param name="fileUrl">File URL to download</param>
        /// <param name="destinationPath">Download file destination path</param>
        /// <param name="progress">(Optionnal) Progress reporting object</param>
        public static async Task DownloadFileAsync(string fileUrl, string destinationPath, IProgress<double>? progress = null)
        {
            using (var client = new HttpClient())
            {
                // Send the request and get the response
                var response = await client.GetAsync(fileUrl, HttpCompletionOption.ResponseHeadersRead);

                response.EnsureSuccessStatusCode();

                // Get the total file size from the response headers
                var totalBytes = response.Content.Headers.ContentLength ?? -1L;

                // Open the response stream and a file stream to write the file
                using (var contentStream = await response.Content.ReadAsStreamAsync())
                using (var fileStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true))
                {
                    var buffer = new byte[8192];
                    long totalRead = 0;
                    int bytesRead;

                    // Read the content in chunks
                    while ((bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                    {
                        await fileStream.WriteAsync(buffer, 0, bytesRead);
                        totalRead += bytesRead;

                        // Progress reporting
                        if (totalBytes > 0 && progress != null)
                        {
                            double percentage = (double)totalRead / totalBytes * 100;
                            progress.Report(percentage);
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Uncompresses a zip file to the specified destination path, with optional progress reporting
        /// </summary>
        /// <param name="archiveFile">Archive file path to uncompress</param>
        /// <param name="destinationPath">Uncompressed files destination path</param>
        /// <param name="progress">(Optionnal) Progress reporting object</param>
        public static async Task UncompressFileAsync(string archiveFile, string destinationPath, IProgress<double>? progress = null)
        {
            using (var archive = ZipFile.OpenRead(archiveFile))
            {
                int totalEntries = archive.Entries.Count; // Total number of entries in the zip file
                int processedEntries = 0;

                foreach (var entry in archive.Entries)
                {
                    // Skip directories
                    if (string.IsNullOrEmpty(entry.Name))
                    {
                        continue;
                    }

                    string fileDestinationPath;
                    fileDestinationPath = Path.Combine(destinationPath, entry.FullName);

                    // Ensure the directory exists
                    Directory.CreateDirectory(Path.GetDirectoryName(fileDestinationPath)!);

                    await Task.Run(() => entry.ExtractToFile(fileDestinationPath, true));

                    // Update progress
                    processedEntries++;

                    if(progress != null)
                     progress.Report((double)processedEntries / totalEntries * 100);
                }
            }
        }

        /// <summary>
        /// Uncompresses a subfolder contained in a zip file to the specified destination path, with optional progress reporting
        /// </summary>
        /// <param name="archiveFile">Archive file path to uncompress</param>
        /// <param name="destinationPath">Uncompressed files destination path</param>
        /// <param name="subFolderName">SubFolder name to uncompress</param>
        /// <param name="progress">(Optionnal) Progress reporting object</param>
        public static async Task UncompressSubFolderInFileAsync(string archiveFile, string destinationPath, string subFolderName, IProgress<double>? progress = null)
        {
            using (var archive = ZipFile.OpenRead(archiveFile))
            {
                // Ensure subfolderName ends with a slash
                if (!subFolderName.EndsWith("/"))
                    subFolderName += "/";

                // Filter entries that are in the specified subfolder
                var entries = archive.Entries
                    .Where(e => e.FullName.StartsWith(subFolderName, StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(e.Name))
                    .ToList();

                int totalEntries = archive.Entries.Count; // Total number of entries in the zip file
                int processedEntries = 0;

                foreach (var entry in archive.Entries)
                {
                    // Skip directories
                    if (string.IsNullOrEmpty(entry.Name))
                    {
                        continue;
                    }

                    // Remove the subfolder prefix from the destination path
                    string relativePath = entry.FullName.Substring(subFolderName.Length);
                    string fileDestinationPath = Path.Combine(destinationPath, relativePath);

                    Directory.CreateDirectory(Path.GetDirectoryName(fileDestinationPath)!);

                    await Task.Run(() => entry.ExtractToFile(fileDestinationPath, true));

                    processedEntries++;
                    if (progress != null)
                        progress.Report((double)processedEntries / totalEntries * 100);
                }
            }
        }
    }
}
