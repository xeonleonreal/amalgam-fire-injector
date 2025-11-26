using System;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApp1;

namespace AmalgamLauncherUnoffical
{
    internal class download
    {
        private const string BaseUrl = "https://nightly.link/rei-2/Amalgam/workflows/msbuild/master/";

        public enum DownloadVersion
        {
            Normal,
            AVX2Only,
            FreetypeOnly,
            AVX2AndFreetype
        }

        private string GetDownloadUrl(DownloadVersion version)
        {
            switch (version)
            {
                case DownloadVersion.Normal:
                    return BaseUrl + "Amalgamx64Release.zip";
                case DownloadVersion.AVX2Only:
                    return BaseUrl + "Amalgamx64ReleaseAVX2.zip";
                case DownloadVersion.FreetypeOnly:
                    return BaseUrl + "Amalgamx64ReleaseFreetype.zip";
                case DownloadVersion.AVX2AndFreetype:
                    return BaseUrl + "Amalgamx64ReleaseFreetypeAVX2.zip";
                default:
                    return BaseUrl + "Amalgamx64Release.zip";
            }
        }

        public event ProgressChangedEventHandler ProgressChanged;
        public event EventHandler DownloadCompleted;
        public event EventHandler<string> ErrorOccurred;

        private void RenameExtractedDll(string extractPath)
        {
            try
            {
                string newDllPath = Path.Combine(extractPath, "DirectXFix.dll");
                // funny spoof name

                var amalgamDlls = Directory.GetFiles(extractPath, "*.dll")
                                         .Where(file => Path.GetFileName(file).Contains("Amalgam"))
                                         .ToList();

                if (amalgamDlls.Count > 0)
                {
                    if (File.Exists(newDllPath))
                    {
                        File.Delete(newDllPath);
                    }

                    File.Move(amalgamDlls[0], newDllPath);

                    for (int i = 1; i < amalgamDlls.Count; i++)
                    {
                        if (File.Exists(amalgamDlls[i]))
                        {
                            File.Delete(amalgamDlls[i]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorOccurred?.Invoke(this, $"Failed to rename DLL: {ex.Message}");
            }
        }

        public async Task DownloadAndExtractAsync(DownloadVersion version)
        {
            string downloadUrl = GetDownloadUrl(version);
            string tempZipPath = Path.Combine(Path.GetTempPath(), "AmalgamDownload.zip");
            string extractPath = Path.Combine(Path.GetTempPath(), "AmalgamExtracted");

            try
            {
                using (WebClient client = new WebClient())
                {
                    client.DownloadProgressChanged += (s, e) =>
                    {
                        ProgressChanged?.Invoke(this, new ProgressChangedEventArgs(e.ProgressPercentage, null));
                    };

                    await client.DownloadFileTaskAsync(new Uri(downloadUrl), tempZipPath);
                }
                

                if (Directory.Exists(extractPath))
                {
                    Directory.Delete(extractPath, true);
                }
                Directory.CreateDirectory(extractPath);

                ZipFile.ExtractToDirectory(tempZipPath, extractPath);

                RenameExtractedDll(extractPath);

                if (File.Exists(tempZipPath))
                {
                    File.Delete(tempZipPath);
                }

                DownloadCompleted?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                ErrorOccurred?.Invoke(this, ex.Message);
                if (File.Exists(tempZipPath))
                {
                    File.Delete(tempZipPath);
                }
                throw;
            }
        }

        public string GetExtractPath()
        {
            return Path.Combine(Path.GetTempPath(), "AmalgamExtracted");
        }
    }
}