using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Collections.Generic;
using WindowsFormsApp4;

public class Injector
{
    [DllImport("kernel32.dll")]
    private static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, int dwProcessId);

    [DllImport("kernel32.dll")]
    private static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

    [DllImport("kernel32.dll")]
    private static extern IntPtr GetModuleHandle(string lpModuleName);

    [DllImport("kernel32.dll")]
    private static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, uint flAllocationType, uint flProtect);

    [DllImport("kernel32.dll")]
    private static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint nSize, out UIntPtr lpNumberOfBytesWritten);

    [DllImport("kernel32.dll")]
    private static extern IntPtr CreateRemoteThread(IntPtr hProcess, IntPtr lpThreadAttributes, uint dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, IntPtr lpThreadId);

    [DllImport("kernel32.dll")]
    private static extern uint WaitForSingleObject(IntPtr hHandle, uint dwMilliseconds);

    [DllImport("kernel32.dll")]
    private static extern bool CloseHandle(IntPtr hObject);

    private const uint PROCESS_ALL_ACCESS = 0x1F0FFF;
    private const uint MEM_COMMIT = 0x1000;
    private const uint MEM_RESERVE = 0x2000;
    private const uint PAGE_READWRITE = 0x04;
    private const uint INFINITE = 0xFFFFFFFF;
    public static string selectedDllPath = "";

    public static bool Inject(string processName = "tf_win64")
    {
        try
        {
            // Step 1: Find the target process
            Process[] processes = Process.GetProcessesByName(Path.GetFileNameWithoutExtension(processName));
            if (processes.Length == 0)
            {
                MessageBox.Show($"Process {processName} not found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            Process targetProcess = processes[0];
            Console.WriteLine($"Found process: {targetProcess.ProcessName} (PID: {targetProcess.Id})");

            // Step 2: Find the amalgamex folder in temp
            string tempPath = Path.GetTempPath();
            string amalgamPath = Path.Combine(tempPath, "amalgamex");

            if (!Directory.Exists(amalgamPath))
            {
                MessageBox.Show($"Folder not found: {amalgamPath}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            Console.WriteLine($"Found amalgamex folder: {amalgamPath}");

            //// Step 3: Find all available DLL files
            //string[] targetDlls = {
            //    "Amalgamx64ReleaseAVX2.dll",
            //    "Amalgamx64Release.dll",
            //    "Amalgamx64ReleaseFreetype.dll",
            //    "Amalgamx64ReleaseFreetypeAVX2.dll"
            //};

            //List<string> foundDlls = new List<string>();
            //foreach (string dllName in targetDlls)
            //{
            //    string fullPath = Path.Combine(amalgamPath, dllName);
            //    if (File.Exists(fullPath))
            //    {
            //        foundDlls.Add(fullPath);
            //        Console.WriteLine($"Found DLL: {dllName}");
            //    }
            //}

            //if (foundDlls.Count == 0)
            //{
            //    MessageBox.Show("No target DLLs found in amalgamex folder!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    return false;
            //}

            // Step 4: Let user select which DLL to inject
            //Form1 form = new Form1();
            //string selectedDllPath = form.checkedListBox1.SelectedItem as string;
            if (string.IsNullOrEmpty(selectedDllPath))
            {
                MessageBox.Show("No DLL selected");
                return false;
            }
            else
            {
                Console.WriteLine($"Selected DLL: {Path.GetFileName(selectedDllPath)}");
            }

            // Step 5: Inject the selected DLL
            return InjectDll(targetProcess, selectedDllPath);
            //return false; debug moment
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Injection failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }
    }

    //private static string SelectDllToInject(List<string> dllPaths)
    //{
    //    if (dllPaths.Count == 1)
    //    {
    //        // If only one DLL found, use it automatically
    //        string singleDll = dllPaths[0];
    //        DialogResult result = MessageBox.Show(
    //            $"Only one DLL found:\n{Path.GetFileName(singleDll)}\n\nInject this DLL?",
    //            "Single DLL Found",
    //            MessageBoxButtons.YesNo,
    //            MessageBoxIcon.Question);

    //        return result == DialogResult.Yes ? singleDll : null;
    //    }
    //    else
    //    {
    //        // If multiple DLLs found, show selection dialog
    //        using (var form = new Form())
    //        using (var listBox = new ListBox())
    //        using (var okButton = new Button())
    //        using (var cancelButton = new Button())
    //        {
    //            form.Text = "Select DLL to Inject";
    //            form.Size = new System.Drawing.Size(400, 300);
    //            form.FormBorderStyle = FormBorderStyle.FixedDialog;
    //            form.StartPosition = FormStartPosition.CenterScreen;
    //            form.MaximizeBox = false;
    //            form.MinimizeBox = false;

    //            // ListBox setup
    //            listBox.Location = new System.Drawing.Point(10, 10);
    //            listBox.Size = new System.Drawing.Size(365, 180);
    //            listBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

    //            foreach (string dllPath in dllPaths)
    //            {
    //                listBox.Items.Add(Path.GetFileName(dllPath));
    //            }
    //            listBox.SelectedIndex = 0;

    //            // OK Button
    //            okButton.Text = "Inject";
    //            okButton.Location = new System.Drawing.Point(200, 200);
    //            okButton.Size = new System.Drawing.Size(75, 30);
    //            okButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
    //            okButton.DialogResult = DialogResult.OK;

    //            // Cancel Button
    //            cancelButton.Text = "Cancel";
    //            cancelButton.Location = new System.Drawing.Point(300, 200);
    //            cancelButton.Size = new System.Drawing.Size(75, 30);
    //            cancelButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
    //            cancelButton.DialogResult = DialogResult.Cancel;

    //            form.Controls.AddRange(new Control[] { listBox, okButton, cancelButton });
    //            form.AcceptButton = okButton;
    //            form.CancelButton = cancelButton;

    //            if (form.ShowDialog() == DialogResult.OK && listBox.SelectedIndex >= 0)
    //            {
    //                return dllPaths[listBox.SelectedIndex];
    //            }
    //        }
    //    }

    //    return null;
    //}

     static bool InjectDll(Process targetProcess, string dllPath)
    {
        IntPtr processHandle = IntPtr.Zero;
        IntPtr allocatedMemory = IntPtr.Zero;
        IntPtr loadLibraryAddr = IntPtr.Zero;
        IntPtr remoteThread = IntPtr.Zero;

        try
        {
            // Open the target process
            processHandle = OpenProcess(PROCESS_ALL_ACCESS, false, targetProcess.Id);
            if (processHandle == IntPtr.Zero)
            {
                MessageBox.Show("Failed to open process!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            // Get address of LoadLibraryA
            loadLibraryAddr = GetProcAddress(GetModuleHandle("kernel32.dll"), "LoadLibraryA");
            if (loadLibraryAddr == IntPtr.Zero)
            {
                MessageBox.Show("Failed to get LoadLibraryA address!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            // Allocate memory in the target process
            uint dllPathSize = (uint)((dllPath.Length + 1) * Marshal.SizeOf(typeof(char)));
            allocatedMemory = VirtualAllocEx(processHandle, IntPtr.Zero, dllPathSize, MEM_COMMIT | MEM_RESERVE, PAGE_READWRITE);
            if (allocatedMemory == IntPtr.Zero)
            {
                MessageBox.Show("Failed to allocate memory in target process!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            // Write DLL path to allocated memory
            byte[] dllPathBytes = System.Text.Encoding.ASCII.GetBytes(dllPath);
            if (!WriteProcessMemory(processHandle, allocatedMemory, dllPathBytes, (uint)dllPathBytes.Length, out UIntPtr bytesWritten))
            {
                MessageBox.Show("Failed to write to process memory!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            // Create remote thread to load the DLL
            remoteThread = CreateRemoteThread(processHandle, IntPtr.Zero, 0, loadLibraryAddr, allocatedMemory, 0, IntPtr.Zero);
            if (remoteThread == IntPtr.Zero)
            {
                MessageBox.Show("Failed to create remote thread!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            // Wait for the thread to complete
            uint result = WaitForSingleObject(remoteThread, INFINITE);
            if (result == 0xFFFFFFFF)
            {
                MessageBox.Show("Thread execution failed!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            string dllName = Path.GetFileName(dllPath);
            MessageBox.Show($"DLL '{dllName}' injected successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return true;
        }
        finally
        {
            // Clean up
            if (remoteThread != IntPtr.Zero) CloseHandle(remoteThread);
            if (allocatedMemory != IntPtr.Zero) CloseHandle(allocatedMemory);
            if (processHandle != IntPtr.Zero) CloseHandle(processHandle);
        }
    }
}