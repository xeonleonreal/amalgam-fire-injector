using System;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Threading.Tasks;
public class Download
{
    private readonly HttpClient _client = new HttpClient();

    private readonly string _extractionDirectory;

    public Download()
    {
        string randomFolderName = "amalgamex";
        _extractionDirectory = Path.Combine(Path.GetTempPath(), randomFolderName);
        if (!Directory.Exists(Path.GetTempPath()))
        {
            Directory.CreateDirectory(Path.GetTempPath());
        }

        Console.WriteLine($"Initialized. Extraction folder: {_extractionDirectory}");
    }
    public async Task DownloadF(string url, bool addV2Suffix = false)
    {
        string tmpPath = Path.GetTempFileName();
        Console.WriteLine($"\nStarting download of: {url}");

        try
        {
            using (HttpResponseMessage response = await _client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead))
            {
                response.EnsureSuccessStatusCode();
                using (Stream contentStream = await response.Content.ReadAsStreamAsync())
                using (FileStream fileStream = new FileStream(tmpPath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    await contentStream.CopyToAsync(fileStream);
                }
            }
            Console.WriteLine("Download complete. Starting extraction...");
            string dir = _extractionDirectory;
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            ZipFile.ExtractToDirectory(tmpPath, dir);

            if (addV2Suffix)
            {
                AddV2SuffixToDlls(dir);
            }

            Console.WriteLine($"Extraction complete. Files are in: {Path.GetFullPath(dir)}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR during operation: {ex.Message}");
        }
        finally
        {
            if (File.Exists(tmpPath))
            {
                File.Delete(tmpPath);
            }
        }
    }
    private void AddV2SuffixToDlls(string directory)
    {
        try
        {
            string[] dllFiles = Directory.GetFiles(directory, "*.dll");

            foreach (string dllFile in dllFiles)
            {
                string fileName = Path.GetFileNameWithoutExtension(dllFile);
                string extension = Path.GetExtension(dllFile);
                string newFileName = fileName + "_V2" + extension;
                string newPath = Path.Combine(Path.GetDirectoryName(dllFile), newFileName);
                File.Move(dllFile, newPath);
                Console.WriteLine($"Renamed: {Path.GetFileName(dllFile)} -> {newFileName}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error renaming DLL files: {ex.Message}");
        }
    }
    public string GetExtractionDirectory()
    {
        return _extractionDirectory;
    }
}
