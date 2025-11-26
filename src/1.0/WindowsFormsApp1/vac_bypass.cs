using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace AmalgamLauncherUnoffical
{
    internal static class vac_bypass
    {
        /// <summary>
        /// Main entry for VAC bypass flow.
        /// - Copies `vac-bypass-loader.exe` from the extractor path to a temp folder.
        /// - Starts the exe and keeps the UI buttons disabled while it runs.
        /// - After the exe exits, waits for Steam to be fully loaded (polls up to timeout).
        /// - Attempts to delete the temp exe and re-enables buttons.
        /// </summary>
        /// <param name="extractSourcePath">Folder where `vac-bypass-loader.exe` was extracted.</param>
        /// <param name="setButtonsEnabled">Delegate to enable/disable the main buttons in the UI.</param>
        /// <param name="steamTimeoutSeconds">How long to wait (seconds) for Steam to appear after bypass exits.</param>
        public static async Task MainAsync(string extractSourcePath, Action<bool> setButtonsEnabled, int steamTimeoutSeconds = 180)
        {
            if (setButtonsEnabled == null) throw new ArgumentNullException(nameof(setButtonsEnabled));
            if (string.IsNullOrWhiteSpace(extractSourcePath)) return;

            string sourceExe = Path.Combine(extractSourcePath, "vac-bypass-loader.exe");
            if (!File.Exists(sourceExe)) return; // nothing to do

            string tempDir = Path.Combine(Path.GetTempPath(), "AmalgamExtracted");
            Directory.CreateDirectory(tempDir);
            string tempExe = Path.Combine(tempDir, "vac-bypass-loader.exe");

            try
            {
                // Copy to temp (overwrite if exists)
                File.Copy(sourceExe, tempExe, true);
            }
            catch
            {
                // copy failed — bail out and re-enable buttons just in case caller disabled them beforehand
                try { setButtonsEnabled(true); } catch { }
                return;
            }

            // Disable main buttons while bypass runs
            try { setButtonsEnabled(false); } catch { }

            Process proc = null;
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = tempExe,
                    WorkingDirectory = tempDir,
                    UseShellExecute = true,
                    CreateNoWindow = true
                };

                proc = Process.Start(psi);
            }
            catch
            {
                // failed to start — re-enable and exit
                try { setButtonsEnabled(true); } catch { }
                return;
            }

            // Wait for bypass process to exit (non-blocking)
            try
            {
                await Task.Run(() =>
                {
                    try { proc?.WaitForExit(); } catch { }
                });
            }
            catch
            {
                // ignore wait errors
            }

            // After the bypass process exits, wait for Steam to be fully loaded (poll MainWindowHandle)
            bool steamFound = false;
            for (int i = 0; i < steamTimeoutSeconds; i++)
            {
                try
                {
                    var steamProcs = Process.GetProcessesByName("steam");
                    if (steamProcs.Any(p => p.MainWindowHandle != IntPtr.Zero))
                    {
                        steamFound = true;
                        break;
                    }
                }
                catch
                {
                    // ignore process enumeration errors
                }
                await Task.Delay(1000);
            }

            // Try to delete the temp exe (best-effort)
            try
            {
                if (File.Exists(tempExe))
                    File.Delete(tempExe);
            }
            catch
            {
                // ignore
            }

            // Re-enable buttons
            try { setButtonsEnabled(true); } catch { }

            // Caller may show notifications if desired based on steamFound
        }

        // New: extract embedded resource (EXE) and run it
        public static async Task RunFromEmbeddedResourceAsync(string resourceName, Action<bool> setButtonsEnabled, int steamTimeoutSeconds = 180)
        {
            if (setButtonsEnabled == null) throw new ArgumentNullException(nameof(setButtonsEnabled));
            if (string.IsNullOrWhiteSpace(resourceName)) throw new ArgumentNullException(nameof(resourceName));

            var asm = Assembly.GetExecutingAssembly();
            string[] names;
            try { names = asm.GetManifestResourceNames(); }
            catch { names = new string[0]; }

            // If resource not found, bail out (caller can inspect names for debugging)
            if (!names.Contains(resourceName))
            {
                // optional: throw or return. We'll return silently.
                return;
            }

            string tempDir = Path.Combine(Path.GetTempPath(), "AmalgamEmbedded");
            Directory.CreateDirectory(tempDir);
            string tempExe = Path.Combine(tempDir, "vac-bypass-loader.exe");

            try
            {
                using (var rs = asm.GetManifestResourceStream(resourceName))
                {
                    if (rs == null) return;
                    using (var fs = new FileStream(tempExe, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        rs.CopyTo(fs);
                    }
                }
            }
            catch
            {
                try { setButtonsEnabled(true); } catch { }
                return;
            }

            await RunExeAndWaitAsync(tempExe, tempDir, setButtonsEnabled, steamTimeoutSeconds);
        }

        // Shared runner: starts exe, waits for exit, polls for Steam, deletes exe, toggles buttons
        private static async Task RunExeAndWaitAsync(string exePath, string workingDirectory, Action<bool> setButtonsEnabled, int steamTimeoutSeconds)
        {
            try { setButtonsEnabled(false); } catch { }

            Process proc = null;
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = exePath,
                    WorkingDirectory = workingDirectory,
                    UseShellExecute = true,
                    CreateNoWindow = true
                };
                proc = Process.Start(psi);
            }
            catch
            {
                try { setButtonsEnabled(true); } catch { }
                return;
            }

            try
            {
                await Task.Run(() => { try { proc?.WaitForExit(); } catch { } });
            }
            catch { }

            bool steamFound = false;
            for (int i = 0; i < steamTimeoutSeconds; i++)
            {
                try
                {
                    var steamProcs = Process.GetProcessesByName("steam");
                    if (steamProcs.Any(p => p.MainWindowHandle != IntPtr.Zero))
                    {
                        steamFound = true;
                        break;
                    }
                }
                catch { }
                await Task.Delay(1000);
            }

            try
            {
                if (File.Exists(exePath)) File.Delete(exePath);
            }
            catch { }

            try { setButtonsEnabled(true); } catch { }
        }
    }
}

// inside Form1 after you know the extract path:

