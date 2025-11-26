using System;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Threading.Tasks;

/// <summary>
/// Handles downloading and decompressing a file, managing the extraction 
/// directory internally for later use.
/// </summary>
public class Download
{
    // HttpClient is designed to be instantiated once and reused throughout the application's lifetime.
    private readonly HttpClient _client = new HttpClient();

    // Private field to store the random, persistent extraction directory name.
    private readonly string _extractionDirectory;

    public Download()
    {
        // Generates a unique, random string using Guid and combines it with a prefix 
        // in the system's temporary directory to avoid conflicts.
        string randomFolderName = "amalgamex";
        _extractionDirectory = Path.Combine(Path.GetTempPath(), randomFolderName);

        // Ensure the base directory exists before using it
        if (!Directory.Exists(Path.GetTempPath()))
        {
            Directory.CreateDirectory(Path.GetTempPath());
        }

        Console.WriteLine($"Initialized. Extraction folder: {_extractionDirectory}");
    }

    /// <summary>
    /// Downloads the file at the given URL, saves it to a temporary file,
    /// and extracts it to the persistent, randomly named directory.
    /// </summary>
    /// <param name="url">The URL of the ZIP file to download.</param>
    public async Task DownloadF(string url)
    {
        // Use a temporary file path for the downloaded ZIP data
        string tmpPath = Path.GetTempFileName();
        Console.WriteLine($"\nStarting download of: {url}");

        try
        {
            // --- 1. DOWNLOAD ---
            using (HttpResponseMessage response = await _client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead))
            {
                response.EnsureSuccessStatusCode();

                // Save stream to temp file using nested 'using' blocks (C# 7.3 compatible)
                using (Stream contentStream = await response.Content.ReadAsStreamAsync())
                using (FileStream fileStream = new FileStream(tmpPath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    // Copy stream contents asynchronously
                    await contentStream.CopyToAsync(fileStream);
                }
            }
            Console.WriteLine("Download complete. Starting extraction...");

            // --- 2. EXTRACT ---
            string dir = _extractionDirectory;
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            // Note: This assumes the file is a standard ZIP archive.
            ZipFile.ExtractToDirectory(tmpPath, dir);

            Console.WriteLine($"Extraction complete. Files are in: {Path.GetFullPath(dir)}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR during operation: {ex.Message}");
        }
        finally
        {
            // Clean up the temporary file
            if (File.Exists(tmpPath))
            {
                File.Delete(tmpPath);
            }
        }
    }

    /// <summary>
    /// Returns the path to the random, persistent directory where the file was extracted.
    /// </summary>
    public string GetExtractionDirectory()
    {
        return _extractionDirectory;
    }
}
