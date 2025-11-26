using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AmalgamLauncherUnoffical
{
    internal class injector
    {
        [DllImport("kernel32.dll")]
        private static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        private static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, uint flAllocationType, uint flProtect);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint nSize, out UIntPtr lpNumberOfBytesWritten);

        [DllImport("kernel32.dll")]
        private static extern IntPtr CreateRemoteThread(IntPtr hProcess, IntPtr lpThreadAttributes, uint dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, IntPtr lpThreadId);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool CloseHandle(IntPtr hObject);

        private const int PROCESS_CREATE_THREAD = 0x0002;
        private const int PROCESS_QUERY_INFORMATION = 0x0400;
        private const int PROCESS_VM_OPERATION = 0x0008;
        private const int PROCESS_VM_WRITE = 0x0020;
        private const int PROCESS_VM_READ = 0x0010;
        private const int INJECT_RIGHTS = PROCESS_CREATE_THREAD | PROCESS_VM_OPERATION | PROCESS_VM_WRITE | 
                                        PROCESS_VM_READ | PROCESS_QUERY_INFORMATION;

        private const uint MEM_COMMIT = 0x00001000;
        private const uint MEM_RESERVE = 0x00002000;
        private const uint PAGE_READWRITE = 4;

        public async Task<bool> InjectDLLAsync(string processName, string dllPath)
        {
            try
            {
                Process targetProcess = null;
                int attempts = 0;
                const int maxAttempts = 300; 

                while (targetProcess == null && attempts < maxAttempts)
                {
                    Process[] processes = Process.GetProcessesByName("tf_win64");
                    
                    if (processes.Length > 0)
                    {
                        targetProcess = processes[0]; 
                        //MessageBox.Show($"Found TF2 process: {targetProcess.ProcessName} (PID: {targetProcess.Id})", "Process Found");
                        break; 
                    }

                    attempts++;
                    await Task.Delay(100); 
                }

                if (targetProcess == null)
                {
                    throw new Exception("Could not find Team Fortress 2 process (tf_win64.exe). Please make sure the game is running.");
                }

                
                IntPtr procHandle = OpenProcess(INJECT_RIGHTS, false, targetProcess.Id);

                if (procHandle == IntPtr.Zero)
                {
                    int error = Marshal.GetLastWin32Error();
                    throw new Exception($"Failed to open process (Error: {error}). Make sure the injector is running as administrator.");
                }

                IntPtr loadLibraryAddr = GetProcAddress(GetModuleHandle("kernel32.dll"), "LoadLibraryA");
                if (loadLibraryAddr == IntPtr.Zero)
                {
                    int error = Marshal.GetLastWin32Error();
                    throw new Exception($"Failed to get LoadLibraryA address (Error: {error}).");
                }

                string absoluteDllPath = Path.GetFullPath(dllPath);
                byte[] dllPathBytes = System.Text.Encoding.ASCII.GetBytes(absoluteDllPath);
                uint pathLength = (uint)(dllPathBytes.Length + 1);

                IntPtr allocMemAddress = VirtualAllocEx(procHandle, IntPtr.Zero, pathLength, 
                    MEM_COMMIT | MEM_RESERVE, PAGE_READWRITE);

                if (allocMemAddress == IntPtr.Zero)
                {
                    int error = Marshal.GetLastWin32Error();
                    throw new Exception($"Memory allocation failed (Error: {error}).");
                }

                UIntPtr bytesWritten;
                if (!WriteProcessMemory(procHandle, allocMemAddress, dllPathBytes, pathLength, out bytesWritten))
                {
                    int error = Marshal.GetLastWin32Error();
                    throw new Exception($"Failed to write memory (Error: {error}).");
                }

                IntPtr threadHandle = CreateRemoteThread(procHandle, IntPtr.Zero, 0, 
                    loadLibraryAddr, allocMemAddress, 0, IntPtr.Zero);

                if (threadHandle == IntPtr.Zero)
                {
                    int error = Marshal.GetLastWin32Error();
                    throw new Exception($"Failed to create thread (Error: {error}).");
                }

                CloseHandle(threadHandle);
                CloseHandle(procHandle);

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Injection failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint nSize, out UIntPtr lpNumberOfBytesRead);
    }
}
